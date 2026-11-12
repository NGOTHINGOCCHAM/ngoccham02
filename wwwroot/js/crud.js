// Buổi 11: CRUD bằng Modal + Ajax (jQuery), dùng chung cho mọi trang có #tableContainer
(function ($) {
    const $modal = $('#crudModal');
    if (!$modal.length) return;
    const modal = bootstrap.Modal.getOrCreateInstance($modal[0]);
    const confirmModal = bootstrap.Modal.getOrCreateInstance(document.getElementById('confirmModal'));
    const token = () => $('#antiForgeryForm input[name="__RequestVerificationToken"]').val();

    // Thông báo nhỏ ở góc phải màn hình
    window.showToast = function (message, ok = true) {
        const $t = $(`<div class="toast align-items-center text-bg-${ok ? 'success' : 'danger'} border-0" role="alert">
            <div class="d-flex"><div class="toast-body"></div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Đóng"></button></div></div>`);
        $t.find('.toast-body').text(message);
        $('#ajaxToasts').append($t);
        $t.on('hidden.bs.toast', () => $t.remove());
        new bootstrap.Toast($t[0], { delay: 4000 }).show();
    };

    function errorMessage(xhr, fallback) {
        if (xhr.status === 404) return 'Không tìm thấy dữ liệu';
        return fallback;
    }

    // Buổi 12: trang hiện tại, thay đổi khi bấm thanh phân trang
    let page = 1;

    // Tải lại bảng dữ liệu, gửi kèm từ khoá, bộ lọc, số dòng mỗi trang (#filterForm) và số trang
    window.reloadTable = function () {
        const $c = $('#tableContainer');
        const query = $('#filterForm').serialize() + '&page=' + page;
        return $.get($c.data('url'), query)
            .done(html => $c.html(html))
            .fail(xhr => $c.html($('<div class="alert alert-danger"></div>').text(errorMessage(xhr, 'Không tải được dữ liệu'))));
    };

    // Form nạp bằng Ajax chưa có validation phía client, cần parse lại
    function parseForm() {
        const $form = $modal.find('form');
        if ($form.length && $.validator && $.validator.unobtrusive) {
            $form.removeData('validator').removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse($form);
        }
        $modal.trigger('form-loaded');
    }

    // Mở modal: Thêm / Sửa / Chi tiết
    $(document).on('click', '[data-modal]', function (e) {
        e.preventDefault();
        $.get($(this).data('modal'))
            .done(html => {
                $modal.find('.modal-content').html(html);
                parseForm();
                modal.show();
            })
            .fail(xhr => showToast(errorMessage(xhr, 'Có lỗi xảy ra'), false));
    });

    // Lưu form trong modal
    $(document).on('submit', '#crudModal form.ajax-form', function (e) {
        e.preventDefault();
        const $form = $(this);
        if ($form.valid && !$form.valid()) return;
        const $btn = $form.find('[type=submit]').prop('disabled', true);
        $.ajax({ url: $form.attr('action'), type: 'POST', data: $form.serialize() })
            .done(res => {
                showToast(res.message, res.success);
                if (res.success) {
                    modal.hide();
                    reloadTable();
                }
            })
            .fail(xhr => {
                if (xhr.status === 400 && xhr.responseText) {
                    // Lỗi validation phía server: hiện lại form kèm thông báo lỗi
                    $modal.find('.modal-content').html(xhr.responseText);
                    parseForm();
                } else {
                    showToast(errorMessage(xhr, 'Có lỗi xảy ra khi lưu'), false);
                }
            })
            .always(() => $btn.prop('disabled', false));
    });

    // Xoá: hỏi lại trong modal xác nhận rồi gửi POST
    let deleteUrl = null;
    $(document).on('click', '[data-delete]', function (e) {
        e.preventDefault();
        deleteUrl = $(this).data('delete');
        $('#confirmText').text(`Bạn có chắc muốn xoá ${$(this).data('name') || 'bản ghi này'}?`);
        bootstrap.Modal.getInstance($modal[0])?.hide();
        confirmModal.show();
    });
    $('#confirmOk').on('click', function () {
        if (!deleteUrl) return;
        $.post(deleteUrl, { __RequestVerificationToken: token() })
            .done(res => {
                showToast(res.message, res.success);
                if (res.success) reloadTable();
            })
            .fail(xhr => showToast(errorMessage(xhr, 'Có lỗi xảy ra khi xoá'), false))
            .always(() => { confirmModal.hide(); deleteUrl = null; });
    });

    // Buổi 12: bấm số trang
    $(document).on('click', '#tableContainer [data-page]', function (e) {
        e.preventDefault();
        if ($(this).closest('.page-item').is('.disabled, .active')) return;
        page = Number($(this).data('page'));
        reloadTable();
    });

    // Buổi 12: tìm kiếm / lọc thì quay về trang 1. Gõ xong 400ms thì tự tìm, không cần bấm nút
    function search() { page = 1; reloadTable(); }
    let timer;
    $('#filterForm').on('submit', e => { e.preventDefault(); search(); });
    $('#filterForm select').on('change', search);
    $('#filterForm input[type=search]').on('input', () => { clearTimeout(timer); timer = setTimeout(search, 400); });

    if ($('#tableContainer').length) reloadTable();
})(jQuery);
