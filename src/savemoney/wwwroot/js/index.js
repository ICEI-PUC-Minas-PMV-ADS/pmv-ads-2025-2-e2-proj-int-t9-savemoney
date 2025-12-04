/* ============================================================================
   INDEX (LISTAGEM DE USUÁRIOS)
   ============================================================================ */

(() => {
    'use strict';

    // Elementos do DOM
    let searchInput = null;
    let usersTable = null;
    let tableRows = null;

    // Estado
    let searchTimeout = null;

    // Inicialização
    document.addEventListener('DOMContentLoaded', initializeIndex);

    function initializeIndex() {
        console.log('Index module loaded');

        searchInput = document.getElementById('searchInput');
        usersTable = document.getElementById('usersTable');

        if (!usersTable) {
            console.warn('Tabela não encontrada - página pode não ter usuários');
            return;
        }

        tableRows = Array.from(usersTable.querySelectorAll('tbody tr'));

        setupEventListeners();
        addAnimationStyles();
    }

    /* Event Listeners
       ======================================================================== */
    function setupEventListeners() {
        // Search input
        if (searchInput) {
            searchInput.addEventListener('input', handleSearchInput);
            searchInput.addEventListener('keydown', handleSearchKeydown);
        }

        // Row click (opcional - expande em mobile)
        if (tableRows) {
            tableRows.forEach(row => {
                row.addEventListener('click', handleRowClick);
            });
        }

        console.log('Event listeners configurados');
    }

    /* Handle Search Input
       ======================================================================== */
    function handleSearchInput(e) {
        // Debounce search
        clearTimeout(searchTimeout);

        searchTimeout = setTimeout(() => {
            const searchTerm = e.target.value.toLowerCase().trim();
            filterTable(searchTerm);
        }, 300);
    }

    /* Handle Search Keydown
       ======================================================================== */
    function handleSearchKeydown(e) {
        if (e.key === 'Escape') {
            searchInput.value = '';
            filterTable('');
            searchInput.blur();
        }
    }

    /* Filter Table
       ======================================================================== */
    function filterTable(searchTerm) {
        if (!tableRows || tableRows.length === 0) return;

        let visibleCount = 0;

        tableRows.forEach(row => {
            const searchData = row.dataset.search?.toLowerCase() || '';

            if (searchTerm === '' || searchData.includes(searchTerm)) {
                row.classList.remove('hidden');
                row.style.animation = 'fadeInRow 0.3s ease';
                visibleCount++;
            } else {
                row.classList.add('hidden');
            }
        });

        // Show/hide empty state
        updateEmptyState(visibleCount);

        console.log(`Filtered: ${visibleCount} of ${tableRows.length} rows visible`);
    }

    /* Update Empty State
       ======================================================================== */
    function updateEmptyState(visibleCount) {
        let emptyState = document.querySelector('.search-empty-state');
        const tableWrapper = usersTable?.closest('.table-wrapper');

        if (visibleCount === 0 && searchInput && searchInput.value.trim() !== '') {
            // Create empty state if doesn't exist
            if (!emptyState && tableWrapper) {
                emptyState = document.createElement('div');
                emptyState.className = 'search-empty-state';
                emptyState.innerHTML = `
                    <span class="material-symbols-outlined empty-icon" aria-hidden="true">search_off</span>
                    <h3 class="empty-title">Nenhum resultado encontrado</h3>
                    <p class="empty-description">Tente buscar com outros termos</p>
                `;
                tableWrapper.parentElement.appendChild(emptyState);
            }
            if (emptyState) emptyState.style.display = 'block';
            if (usersTable) usersTable.style.display = 'none';
        } else {
            if (emptyState) {
                emptyState.style.display = 'none';
            }
            if (usersTable) usersTable.style.display = 'table';
        }
    }

    /* Handle Row Click (Mobile Expand)
       ======================================================================== */
    function handleRowClick(e) {
        // Evita conflito com botões de ação
        if (e.target.closest('.btn-action')) {
            return;
        }

        // Em mobile, adiciona classe active para expandir
        if (window.innerWidth <= 768) {
            const row = e.currentTarget;
            const isActive = row.classList.contains('active');

            // Remove active de todas as rows
            tableRows.forEach(r => r.classList.remove('active'));

            // Toggle current row
            if (!isActive) {
                row.classList.add('active');
            }
        }
    }

    /* Sort Table (Optional Enhancement)
       ======================================================================== */
    function sortTable(columnIndex, direction = 'asc') {
        if (!usersTable) return;

        const tbody = usersTable.querySelector('tbody');
        if (!tbody) return;

        const sortedRows = [...tableRows].sort((a, b) => {
            const aValue = a.cells[columnIndex]?.textContent.trim() || '';
            const bValue = b.cells[columnIndex]?.textContent.trim() || '';

            if (direction === 'asc') {
                return aValue.localeCompare(bValue);
            } else {
                return bValue.localeCompare(aValue);
            }
        });

        // Re-append rows in sorted order
        sortedRows.forEach(row => tbody.appendChild(row));
    }

    /* Animation Styles
       ======================================================================== */
    function addAnimationStyles() {
        if (document.getElementById('index-animations')) {
            return;
        }

        const style = document.createElement('style');
        style.id = 'index-animations';
        style.textContent = `
            /* Fade In Row */
            @keyframes fadeInRow {
                from {
                    opacity: 0;
                    transform: translateX(-0.5rem);
                }
                to {
                    opacity: 1;
                    transform: translateX(0);
                }
            }

            /* Hidden Row */
            .data-table tbody tr.hidden {
                display: none;
            }

            /* Active Row (Mobile) */
            .data-table tbody tr.active {
                background: rgba(59, 130, 246, 0.1);
                border-left: 0.25rem solid var(--index-accent, #3b82f6);
            }

            /* Search Empty State */
            .search-empty-state {
                display: none;
                text-align: center;
                padding: 4rem 2rem;
            }

            .search-empty-state .empty-icon {
                font-size: 5rem;
                color: var(--text-secondary);
                opacity: 0.5;
                margin-bottom: 1.5rem;
                display: block;
            }

            .search-empty-state .empty-title {
                font-size: 1.5rem;
                font-weight: 700;
                color: var(--text-primary);
                margin: 0 0 0.5rem 0;
            }

            .search-empty-state .empty-description {
                font-size: 1rem;
                color: var(--text-secondary);
                margin: 0;
            }

            /* Search Highlight */
            mark {
                background: rgba(59, 130, 246, 0.3);
                color: var(--text-primary);
                border-radius: 0.25rem;
                padding: 0.125rem 0.25rem;
            }

            /* Hover Effects */
            @media (hover: hover) {
                .btn-action:hover {
                    transform: translateY(-0.125rem) scale(1.05);
                }

                .stat-card:hover {
                    transform: translateY(-0.25rem);
                }
            }

            /* Pulse Animation for Stats */
            .stat-card {
                animation: fadeInUp 0.6s ease backwards;
            }

            .stat-card:nth-child(1) {
                animation-delay: 0.1s;
            }

            .stat-card:nth-child(2) {
                animation-delay: 0.2s;
            }

            .stat-card:nth-child(3) {
                animation-delay: 0.3s;
            }

            @keyframes fadeInUp {
                from {
                    opacity: 0;
                    transform: translateY(1rem);
                }
                to {
                    opacity: 1;
                    transform: translateY(0);
                }
            }

            /* Mobile Row Expansion */
            @media (max-width: 48rem) {
                .data-table tbody tr {
                    cursor: pointer;
                }

                .data-table tbody tr.active .hide-mobile {
                    display: table-cell;
                }
            }
        `;
        document.head.appendChild(style);

        console.log('Animation styles adicionados');
    }

    /* Keyboard Navigation
       ======================================================================== */
    document.addEventListener('keydown', (e) => {
        // CTRL + K ou CMD + K para focar no search
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            if (searchInput) {
                searchInput.focus();
                searchInput.select();
            }
        }
    });

    /* Export para debugging
       ======================================================================== */
    window.IndexModule = {
        filterTable,
        sortTable,
        get searchInput() { return searchInput; },
        get tableRows() { return tableRows; }
    };

})();