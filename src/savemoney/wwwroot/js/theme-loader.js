// ============================================
// THEME LOADER - CARREGA TEMA EM TODAS AS PÁGINAS
// ============================================

(function () {
    'use strict';

    // Carregar tema ao iniciar
    document.addEventListener('DOMContentLoaded', carregarTemaGlobal);

    async function carregarTemaGlobal() {
        try {
            // Verificar se tem tema customizado ativo
            const response = await fetch('/Theme/ObterTemaAtivo');
            const tema = await response.json();

            if (tema.id) {
                // Aplicar tema customizado
                aplicarCoresCustomizadas(tema);
                console.log('✅ Tema customizado aplicado:', tema.nomeTema);
            } else {
                // Aplicar tema pré-definido do localStorage
                const temaSalvo = localStorage.getItem('tema-atual');
                if (temaSalvo && temaSalvo !== 'default') {
                    carregarCssTema(temaSalvo);
                    console.log('✅ Tema pré-definido aplicado:', temaSalvo);
                } else {
                    console.log('✅ Tema padrão (default) aplicado');
                }
            }
        } catch (error) {
            console.error('⚠️ Erro ao carregar tema:', error);
        }
    }

    function aplicarCoresCustomizadas(tema) {
        const root = document.documentElement;
        root.style.setProperty('--bg-primary', tema.bgPrimary);
        root.style.setProperty('--bg-secondary', tema.bgSecondary);
        root.style.setProperty('--bg-card', tema.bgCard);
        root.style.setProperty('--border-color', tema.borderColor);
        root.style.setProperty('--text-primary', tema.textPrimary);
        root.style.setProperty('--text-secondary', tema.textSecondary);
        root.style.setProperty('--accent-primary', tema.accentPrimary);
        root.style.setProperty('--accent-primary-hover', tema.accentPrimaryHover);
        root.style.setProperty('--btn-primary-text', tema.btnPrimaryText);

        // Remover CSS de tema pré-definido (se existir)
        const temaAnterior = document.getElementById('tema-css');
        if (temaAnterior) temaAnterior.remove();

        localStorage.removeItem('tema-atual');
    }

    function carregarCssTema(nomeTema) {
        // Remover tema anterior
        const temaAnterior = document.getElementById('tema-css');
        if (temaAnterior) temaAnterior.remove();

        // Adicionar novo tema
        const link = document.createElement('link');
        link.id = 'tema-css';
        link.rel = 'stylesheet';
        link.href = `/css/themes/theme-${nomeTema}.css`;
        document.head.appendChild(link);
    }

})();