(function () {

    const cssVariableBreakpointLG = getComputedStyle(document.documentElement).getPropertyValue('--theme-breakpoint-xl') || '1140px';
    const breakpoint = window.matchMedia(`(max-width: ${cssVariableBreakpointLG})`);
    const CLOSED_STATE_CSS_CLASS = 'menu-hidden';
    const OVERLAY_STATE_CSS_CLASS = 'menu-overlay';
    const TRANSITION_CSS_CLASS = 'has-transition';
    const CSS_ANIMATION_DURATION = 750;
    const menuToggle = document.querySelector('.menu-toggle');
    const menuOverlay = document.querySelector('.menu-overlay-bg');
    const closeMenuBtn = document.querySelector('.close-menu');

    // Check if sidebar should be open or closed
    function checkIfCloseSidebar() {
        const breakpointWidth = parseInt(cssVariableBreakpointLG) || 1140;

        return window.innerWidth > breakpointWidth;
    }

    // Open or close sidebar
    function sidebarCloseOrOpen() {

        if (checkIfCloseSidebar()) {
            document.body.classList.remove(CLOSED_STATE_CSS_CLASS);
            return;
        }

        document.body.classList.add(CLOSED_STATE_CSS_CLASS);
        document.body.classList.remove(OVERLAY_STATE_CSS_CLASS);
        document.body.classList.remove(TRANSITION_CSS_CLASS);
    }

    function addTempBodyTransitionCSSClass() {
        document.body.classList.add(TRANSITION_CSS_CLASS);

        setTimeout(() => {
            document.body.classList.remove(TRANSITION_CSS_CLASS);
        }, CSS_ANIMATION_DURATION);
    }

    // handle menu toggle clicks
    function handleMenuToggleClick() {
        addTempBodyTransitionCSSClass();

        if (!checkIfCloseSidebar()) {
            document.body.classList.toggle(OVERLAY_STATE_CSS_CLASS);
            return;
        }

        document.body.classList.toggle(CLOSED_STATE_CSS_CLASS);
    }

    function closeSidebar() {
        addTempBodyTransitionCSSClass();
        document.body.classList.remove(OVERLAY_STATE_CSS_CLASS);
    }


    // Monitor our breakpoint for changes to device width
    breakpoint.addListener(sidebarCloseOrOpen);

    // add eventlistener for menu toggle & overlay & close menu btn
    if (menuToggle) {
        menuToggle.addEventListener('click', handleMenuToggleClick);
    }

    if (menuOverlay) {
        menuOverlay.addEventListener('click', () => {
            closeSidebar();
        });
    }

    if (closeMenuBtn) {
        closeMenuBtn.addEventListener('click', () => {
            closeSidebar();
        });
    }


    document.addEventListener('DOMContentLoaded', function () {

        // on page load, check if sidebar should be closed
        sidebarCloseOrOpen();

    });

})();
(function () {

    const menuToggle = document.querySelector('.menu-toggle');
    const cssVariableBreakpointLG = getComputedStyle(document.documentElement).getPropertyValue('--theme-breakpoint-xl') || '1140px';
    const breakpoint = window.matchMedia(`(max-width: ${cssVariableBreakpointLG})`);
    const pageURL = getURLSegment(window.location.href);
    const ACTIVE_MENU_CSS_CLASS = 'active';
    const COLLAPSE_ACTIVE_CSS_CLASS = 'show';
    const COLLAPSE_TRIGGER_CSS_CLASS = 'collapsed';

    // hide all drop down menus when sidebar is collapsed or hidden
    function hideDropDownChildMenus() {
        const dropDowns = document.querySelectorAll('.aside .collapse.show') || [];

        dropDowns.forEach((dropdown) => {
            const parent = dropdown.closest('.menu-item');
            const dropdownToggle = parent.querySelector('a[data-bs-toggle]');
            dropdown.classList.remove('show');

            if (dropdownToggle) {
                dropdownToggle.setAttribute('aria-expaned', false);
                dropdownToggle.classList.add('collapsed');
            }
        });
    }

    // get last segment of URL
    function getURLSegment(url) {
        return url.substr(url.lastIndexOf('/') + 1)
            .replace(' ', '')
            .replace(/%20/g, '')
            .trim()
            .toLowerCase();
    }

    //set active sidebar menu item
    function setActiveSidebarMenu({ activeMenu }) {
        const parent = activeMenu.closest('.menu-item');

        activeMenu.classList.add('active');

        if (parent) {
            const menuCollapse = parent.querySelector('.collapse');
            const menuCollapseTrigger = parent.querySelector('a[data-bs-toggle]');

            if (menuCollapse) {
                menuCollapse.classList.add(COLLAPSE_ACTIVE_CSS_CLASS);
            }

            if (menuCollapseTrigger) {
                menuCollapseTrigger.classList.remove(COLLAPSE_TRIGGER_CSS_CLASS);
                menuCollapseTrigger.setAttribute('aria-expanded', true);
            }
        }

    }

    function checkForActiveSidebarMenu() {
        const menuItems = document.querySelectorAll('.aside ul a');

        menuItems.forEach((item) => {
            const itemURL = getURLSegment(item.href);
            if (itemURL === pageURL) {
                setActiveSidebarMenu({ activeMenu: item });
            }
        })
    }

    if (menuToggle) {
        menuToggle.addEventListener('click', () => {
            hideDropDownChildMenus();
        });
    }

    // Monitor our breakpoint for changes to device width
    breakpoint.addListener(hideDropDownChildMenus);
    breakpoint.addListener(checkForActiveSidebarMenu);

    // call func to set active menu on page load
    document.addEventListener('DOMContentLoaded', () => {
        checkForActiveSidebarMenu();
    });

})();
(function () {

    const searchTrigger = document.querySelector('.btn-search');
    const searchBar = document.querySelector('.navbar-search');
    const searchInput = document.querySelector('.navbar-search input');
    const closeSearch = document.querySelector('.close-search');
    const ACTIVE_SEARCH_CSS_CLASS = 'search-active';
    const HIDDEN_CSS_CLASS = 'd-none';

    function toggleSearch() {

        if (document.body.classList.contains(ACTIVE_SEARCH_CSS_CLASS)) {
            document.body.classList.remove(ACTIVE_SEARCH_CSS_CLASS);
            return;
        }

        searchBar.classList.remove(HIDDEN_CSS_CLASS);
        setTimeout(() => {
            document.body.classList.add(ACTIVE_SEARCH_CSS_CLASS);
            if (searchInput) {
                searchInput.focus();
            }
        }, 150);
    }

    if (searchTrigger) {
        searchTrigger.addEventListener('click', function () {
            toggleSearch();
        });
    }

    if (closeSearch) {
        closeSearch.addEventListener('click', function () {
            toggleSearch();
        });
    }

})();
(function () {

    const menuToggle = document.querySelector('.menu-toggle');
    const cssVariableBreakpointLG = getComputedStyle(document.documentElement).getPropertyValue('--theme-breakpoint-xl') || '1140px';
    const breakpoint = window.matchMedia(`(max-width: ${cssVariableBreakpointLG})`);
    const pageURL = getURLSegment(window.location.href);
    const ACTIVE_MENU_CSS_CLASS = 'active';
    const COLLAPSE_ACTIVE_CSS_CLASS = 'show';
    const COLLAPSE_TRIGGER_CSS_CLASS = 'collapsed';

    // hide all drop down menus when sidebar is collapsed or hidden
    function hideDropDownChildMenus() {
        const dropDowns = document.querySelectorAll('.aside .collapse.show') || [];

        dropDowns.forEach((dropdown) => {
            const parent = dropdown.closest('.menu-item');
            const dropdownToggle = parent.querySelector('a[data-bs-toggle]');
            dropdown.classList.remove('show');

            if (dropdownToggle) {
                dropdownToggle.setAttribute('aria-expaned', false);
                dropdownToggle.classList.add('collapsed');
            }
        });
    }

    // get last segment of URL
    function getURLSegment(url) {
        return url.substr(url.lastIndexOf('/') + 1)
            .replace(' ', '')
            .replace(/%20/g, '')
            .trim()
            .toLowerCase();
    }

    //set active sidebar menu item
    function setActiveSidebarMenu({ activeMenu }) {
        const parent = activeMenu.closest('.menu-item');

        activeMenu.classList.add('active');

        if (parent) {
            const menuCollapse = parent.querySelector('.collapse');
            const menuCollapseTrigger = parent.querySelector('a[data-bs-toggle]');

            if (menuCollapse) {
                menuCollapse.classList.add(COLLAPSE_ACTIVE_CSS_CLASS);
            }

            if (menuCollapseTrigger) {
                menuCollapseTrigger.classList.remove(COLLAPSE_TRIGGER_CSS_CLASS);
                menuCollapseTrigger.setAttribute('aria-expanded', true);
            }
        }

    }

    function checkForActiveSidebarMenu() {
        const menuItems = document.querySelectorAll('.aside ul a');

        menuItems.forEach((item) => {
            const itemURL = getURLSegment(item.href);
            if (itemURL === pageURL) {
                setActiveSidebarMenu({ activeMenu: item });
            }
        })
    }

    if (menuToggle) {
        menuToggle.addEventListener('click', () => {
            hideDropDownChildMenus();
        });
    }

    // Monitor our breakpoint for changes to device width
    breakpoint.addListener(hideDropDownChildMenus);
    breakpoint.addListener(checkForActiveSidebarMenu);

    // call func to set active menu on page load
    document.addEventListener('DOMContentLoaded', () => {
        checkForActiveSidebarMenu();
    });

})();
document.addEventListener("DOMContentLoaded", function () {
    var dropdownTriggerList = [].slice.call(document.querySelectorAll('.dropdown-toggle'));
    dropdownTriggerList.forEach(function (dropdownTrigger) {
        new bootstrap.Dropdown(dropdownTrigger);
    });
});