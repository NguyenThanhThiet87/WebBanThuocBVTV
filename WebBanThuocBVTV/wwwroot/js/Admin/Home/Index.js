var modalElement = document.getElementById('detail_orderModal');
var modal = new bootstrap.Modal(modalElement);
var model_container = document.getElementById("modal-container");

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
                            label: 'Doanh thu 6 tháng gần nhất',
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

var detailOrderHome = function (maDonHang) {
    console.log("kkk")
    $.ajax({
        url: "/Admin/ManageOrder/DetailOrder",
        method: "post",
        data: { maDh: maDonHang },
        success: function (res) {
            model_container.innerHTML = res;
            modal.show()
        },
        error: function (res) {
            console.log(res);
        }
    });
}