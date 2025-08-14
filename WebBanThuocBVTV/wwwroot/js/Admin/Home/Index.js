document.addEventListener('DOMContentLoaded', function () {
    const ctx = document.getElementById('revenueChart');
    $.ajax({
        url: "/Admin/Home/RevenueClostSixMonth",
        method: "GET",
        success: function (res) {
            if (res != null) {
                data = []
                for (var item of res) {
                    data.push(item.value)
                }
                data = data.reverse();
                console.log(data);
                new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6'],
                        datasets: [{
                            label: 'Doanh thu',
                            data: data,
                            borderWidth: 1
                        }]
                    },
                    options: {
                        scales: {
                            y: {
                                beginAtZero: true
                            }
                        }
                    }
                });
            }
        },
        error: function (error) {
            console.log(error);
        }
    }
    )
})


