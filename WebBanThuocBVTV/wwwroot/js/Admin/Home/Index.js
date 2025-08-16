document.addEventListener('DOMContentLoaded', function () {
    const ctx = document.getElementById('revenueChart');
    $.ajax({
        url: "/Admin/Home/RevenueClostSixMonth",
        method: "GET",
        success: function (res) {
            if (res != null) {
                datetime = []
                data = []
                for (var item of res) {
                    data.push(item.value)
                    var date = new Date(item.key)
                    datetime.push(date.getFullYear()+ "/" + date.getMonth())
                }
                data = data;

                console.log(data);
                new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: datetime,
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


