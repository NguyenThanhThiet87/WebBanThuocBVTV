function moveTop() {
    const scroller = document.getElementById('scroll-area');      // vùng cuộn riêng
    scroller.scrollTo({ top: 0, behavior: 'smooth' });
}