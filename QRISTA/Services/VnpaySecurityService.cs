using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace QRB.Services
{
    public interface IVnpaySecurityService
    {
        bool ValidateReturnRequest(Dictionary<string, string> parameters, string clientIp);
        bool IsValidTransaction(string transactionId);
        void MarkTransactionAsProcessed(string transactionId);
    }

    public class VnpaySecurityService : IVnpaySecurityService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<VnpaySecurityService> _logger;
        private readonly ConcurrentDictionary<string, DateTime> _processedTransactions;
        
        // VNPAY official IP ranges (cần cập nhật theo documentation mới nhất)
        private readonly HashSet<string> _allowedIPs = new HashSet<string>
        {
            "103.220.87.0/24",
            "103.220.88.0/24", 
            "103.255.51.0/24",
            "117.6.135.0/24",
            "127.0.0.1", // localhost for testing
            "::1" // IPv6 localhost
        };

        public VnpaySecurityService(IMemoryCache cache, ILogger<VnpaySecurityService> logger)
        {
            _cache = cache;
            _logger = logger;
            _processedTransactions = new ConcurrentDictionary<string, DateTime>();
        }

        public bool ValidateReturnRequest(Dictionary<string, string> parameters, string clientIp)
        {
            try
            {
                // 1. Validate IP address
                if (!IsValidSourceIP(clientIp))
                {
                    _logger.LogWarning("Invalid source IP: {ClientIP}", clientIp);
                    return false;
                }

                // 2. Check for required parameters
                if (!parameters.ContainsKey("vnp_SecureHash") || 
                    !parameters.ContainsKey("vnp_TxnRef") ||
                    !parameters.ContainsKey("vnp_ResponseCode"))
                {
                    _logger.LogWarning("Missing required parameters in VNPAY return");
                    return false;
                }

                // 3. Check transaction timestamp (if available)
                if (parameters.ContainsKey("vnp_PayDate"))
                {
                    if (!IsValidTimestamp(parameters["vnp_PayDate"]))
                    {
                        _logger.LogWarning("Invalid or expired timestamp: {PayDate}", parameters["vnp_PayDate"]);
                        return false;
                    }
                }

                // 4. Rate limiting check
                string cacheKey = $"vnpay_rate_limit_{clientIp}";
                if (_cache.TryGetValue(cacheKey, out int requestCount))
                {
                    if (requestCount > 10) // Max 10 requests per minute per IP
                    {
                        _logger.LogWarning("Rate limit exceeded for IP: {ClientIP}", clientIp);
                        return false;
                    }
                    _cache.Set(cacheKey, requestCount + 1, TimeSpan.FromMinutes(1));
                }
                else
                {
                    _cache.Set(cacheKey, 1, TimeSpan.FromMinutes(1));
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating VNPAY return request");
                return false;
            }
        }

        public bool IsValidTransaction(string transactionId)
        {
            // Kiểm tra xem transaction đã được xử lý chưa (chống replay attack)
            return !_processedTransactions.ContainsKey(transactionId);
        }

        public void MarkTransactionAsProcessed(string transactionId)
        {
            _processedTransactions.TryAdd(transactionId, DateTime.Now);
            
            // Cleanup old transactions (older than 24 hours)
            var cutoffTime = DateTime.Now.AddHours(-24);
            var expiredKeys = _processedTransactions
                .Where(kvp => kvp.Value < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();
                
            foreach (var key in expiredKeys)
            {
                _processedTransactions.TryRemove(key, out _);
            }
        }

        private bool IsValidSourceIP(string clientIp)
        {
            // For development/testing, allow localhost
            if (clientIp == "127.0.0.1" || clientIp == "::1" || clientIp.StartsWith("192.168."))
                return true;

            // TODO: Implement proper IP range validation for VNPAY IPs
            // This is a simplified version - in production, use proper CIDR validation
            foreach (var allowedRange in _allowedIPs)
            {
                if (allowedRange.Contains('/'))
                {
                    // CIDR range - implement proper validation
                    continue;
                }
                
                if (clientIp == allowedRange)
                    return true;
            }

            return false;
        }

        private bool IsValidTimestamp(string payDate)
        {
            try
            {
                // VNPAY format: yyyyMMddHHmmss
                if (DateTime.TryParseExact(payDate, "yyyyMMddHHmmss", null, 
                    System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    // Check if timestamp is within reasonable range (not older than 1 hour, not in future)
                    var now = DateTime.Now;
                    return parsedDate >= now.AddHours(-1) && parsedDate <= now.AddMinutes(5);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
