// Common loader: show during DB/async tasks (centered on screen)
(function () {
  var loaderEl = document.getElementById('commonLoader');
  var loaderCount = 0;

  function showLoader() {
    if (!loaderEl) return;
    loaderCount++;
    loaderEl.classList.add('common-loader-show');
  }

  function hideLoader() {
    if (!loaderEl) return;
    loaderCount = loaderCount > 0 ? loaderCount - 1 : 0;
    if (loaderCount === 0) {
      loaderEl.classList.remove('common-loader-show');
    }
  }

  window.showLoader = showLoader;
  window.hideLoader = hideLoader;

  // Form submit: show loader for full-page POSTs (skip forms with data-no-loader)
  document.addEventListener('submit', function (e) {
    var form = e.target;
    if (form && form.tagName === 'FORM' && !form.getAttribute('data-no-loader')) {
      var method = (form.getAttribute('method') || 'get').toLowerCase();
      if (method === 'post' || method === 'put' || method === 'delete') {
        showLoader();
      }
    }
  }, true);

  // Link click: show loader when navigating to another page (same app)
  document.addEventListener('click', function (e) {
    var a = e.target && (e.target.closest ? e.target.closest('a') : (function (n) {
      while (n && n !== document) {
        if (n.tagName === 'A') return n;
        n = n.parentNode;
      }
      return null;
    })(e.target));
    if (!a || !a.href) return;
    if (a.target === '_blank' || a.hasAttribute('download')) return;
    var href = (a.getAttribute('href') || '').trim();
    if (!href || href === '#' || href.indexOf('javascript:') === 0 || href.indexOf('mailto:') === 0 || href.indexOf('tel:') === 0) return;
    if (href.indexOf('http') === 0 && a.origin !== window.location.origin) return;
    showLoader();
  }, true);

  // Wrap fetch so AJAX calls show/hide loader
  var originalFetch = window.fetch;
  if (typeof originalFetch === 'function') {
    window.fetch = function () {
      var args = arguments;
      var url = typeof args[0] === 'string' ? args[0] : (args[0] && args[0].url);
      var opts = args[1] || {};
      var skipLoader = opts && (opts.headers && opts.headers['X-Skip-Loader'] !== undefined);
      if (!skipLoader) showLoader();
      return originalFetch.apply(this, args).then(function (response) {
        if (!skipLoader) hideLoader();
        return response;
      }).catch(function (err) {
        if (!skipLoader) hideLoader();
        throw err;
      });
    };
  }
})();
