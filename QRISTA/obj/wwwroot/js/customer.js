$(document).ready(function () {
    // Lấy danh sách khách hàng (render sẵn)
    let allRows = $('#customerTableBody tr');
    let pageSize = 10;
    let currentPage = 1;

    function renderTable() {
        let search = $('#searchCustomer').val().toLowerCase();
        let status = $('#filterStatus').val();
        let rows = allRows.filter(function () {
            let t = $(this).text().toLowerCase();
            // Trạng thái sẽ lấy từ class hoặc data, ở đây demo chỉ lọc text
            let matchStatus = true;
            if (status === '1') matchStatus = $(this).find('td:last').text().includes('Đã xóa');
            if (status === '0') matchStatus = !$(this).find('td:last').text().includes('Đã xóa');
            return t.indexOf(search) > -1 && matchStatus;
        });
        let total = rows.length;
        let totalPages = Math.ceil(total / pageSize) || 1;
        if (currentPage > totalPages) currentPage = 1;
        $('#customerTableBody tr').hide();
        rows.slice((currentPage - 1) * pageSize, currentPage * pageSize).show();
        // Phân trang hiện đại
        let pag = '';
        for (let i = 1; i <= totalPages; i++) {
            pag += `<button class="${i === currentPage ? 'active' : ''}" data-page="${i}">${i}</button>`;
        }
        $('#customerPagination').html(pag);
    }
    $('#searchCustomer, #filterStatus').on('input change', function () {
        currentPage = 1;
        renderTable();
    });
    $('#customerPagination').on('click', 'button', function () {
        currentPage = parseInt($(this).data('page'));
        renderTable();
    });
    renderTable();

    // Thêm khách hàng
    $('#btnAddCustomer').click(function () {
        $('#customerForm')[0].reset();
        $('#customerId').val('');
        $('#customerModalTitle').text('Thêm khách hàng');
        $('#customerModal').modal('show');
    });
    // Sửa khách hàng
    $(document).on('click', '.btnEditCustomer', function () {
        let row = $(this).closest('tr');
        $('#customerId').val($(this).data('id'));
        $('#tenKhachHang').val(row.find('td:eq(1)').text());
        $('#soDienThoai').val(row.find('td:eq(2)').text());
        $('#giaTriDonHang').val(row.find('td:eq(3)').text());
        // Trạng thái demo: nếu có class hoặc text "Đã xóa" thì set 1
        $('#isDelete').val(row.find('td:last').text().includes('Đã xóa') ? '1' : '0');
        $('#customerModalTitle').text('Sửa khách hàng');
        $('#customerModal').modal('show');
    });
    // Lưu khách hàng
    $('#customerForm').submit(function (e) {
        e.preventDefault();
        let id = $('#customerId').val();
        let url = id ? '/Customer/UpdateCustomer' : '/Customer/AddCustomer';
        let data = {
            Id: id,
            TenKhachHang: $('#tenKhachHang').val(),
            SoDienThoai: $('#soDienThoai').val(),
            GiaTriDonHang: $('#giaTriDonHang').val(),
            IsDelete: $('#isDelete').val() === '1'
        };
        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            success: function (res) {
                if (res.success) location.reload();
                else alert('Có lỗi xảy ra!');
            },
            error: function () { alert('Có lỗi xảy ra!'); }
        });
    });
    // Xóa khách hàng
    let deleteId = null;
    $(document).on('click', '.btnDeleteCustomer', function () {
        deleteId = $(this).data('id');
        $('#deleteCustomerModal').modal('show');
    });
    $('#confirmDeleteCustomer').click(function () {
        if (!deleteId) return;
        $.ajax({
            url: '/Customer/DeleteCustomer',
            type: 'POST',
            data: { id: deleteId },
            success: function (res) {
                if (res.success) location.reload();
                else alert('Không thể xóa!');
            },
            error: function () { alert('Có lỗi xảy ra!'); }
        });
    });
});
