// Dashboard JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Initialize charts
    initUserRoleChart();
    initPopularCoursesChart();
    
    // Tab functionality
    initTabs();
});

function initUserRoleChart() {
    const ctx = document.getElementById('userRoleChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Quản trị viên', 'Người dùng', 'Điều hành viên'],
            datasets: [{
                data: [5, 50, 15],
                backgroundColor: [
                    '#4285f4',
                    '#34a853', 
                    '#fbbc05'
                ],
                borderWidth: 0,
                cutout: '60%'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((context.parsed * 100) / total).toFixed(1);
                            return `${context.label}: ${percentage}%`;
                        }
                    }
                }
            }
        }
    });
}

function initPopularCoursesChart() {
    const ctx = document.getElementById('popularCoursesChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Kinh tế chính trị', 'Lập trình Web', 'Cơ sở dữ liệu'],
            datasets: [{
                label: 'Số người học',
                data: [45, 3, 1],
                backgroundColor: '#4285f4',
                borderRadius: 6,
                maxBarThickness: 40
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 15
                    },
                    grid: {
                        color: '#f0f2f5'
                    }
                },
                x: {
                    grid: {
                        display: false
                    }
                }
            }
        }
    });
}

function initTabs() {
    const tabBtns = document.querySelectorAll('.tab-btn');
    
    tabBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            // Remove active class from all tabs
            tabBtns.forEach(tab => tab.classList.remove('active'));
            
            // Add active class to clicked tab
            this.classList.add('active');
            
            // Here you can add logic to show/hide different chart content
            // based on the selected tab
        });
    });
}

// Update date functionality
document.querySelector('.update-btn')?.addEventListener('click', function() {
    // Add loading state
    this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang cập nhật...';
    
    // Simulate API call
    setTimeout(() => {
        this.innerHTML = '<i class="fas fa-sync-alt"></i> Cập nhật';
        
        // Here you would typically reload the charts with new data
        console.log('Data updated for date range');
    }, 1000);
});

// Notification click handler
document.querySelector('.notification-btn')?.addEventListener('click', function() {
    alert('Không có thông báo mới');
});

// Mobile sidebar toggle (if needed)
function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    sidebar.classList.toggle('show');
}
