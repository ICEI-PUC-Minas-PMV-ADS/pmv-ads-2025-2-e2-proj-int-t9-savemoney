/* ============================================================================
   EDUCACAO FINANCEIRA - HUB DE CONHECIMENTO
   ============================================================================ */

(() => {
    'use strict';

    // Inicializacao
    document.addEventListener('DOMContentLoaded', initializeEducacao);

    function initializeEducacao() {
        console.log('Educacao Financeira module loaded');

        // Lazy loading de imagens
        setupLazyLoading();

        // Parallax suave no background (se nao houver prefers-reduced-motion)
        if (!window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
            setupParallax();
        }

        // Animacao de entrada dos cards ao scroll
        setupScrollAnimations();
    }

    /* Lazy Loading de Imagens
       ======================================================================== */
    function setupLazyLoading() {
        const images = document.querySelectorAll('img[loading="lazy"]');

        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;

                        // Adiciona classe de fade-in quando carrega
                        img.addEventListener('load', () => {
                            img.style.opacity = '1';
                        });

                        img.style.opacity = '0';
                        img.style.transition = 'opacity 0.3s ease';

                        observer.unobserve(img);
                    }
                });
            }, {
                rootMargin: '50px'
            });

            images.forEach(img => imageObserver.observe(img));
        }
    }

    /* Parallax Suave no Background
       ======================================================================== */
    function setupParallax() {
        const background = document.querySelector('.background-fixed-image');
        if (!background) return;

        let ticking = false;

        function updateParallax() {
            const scrolled = window.scrollY; // CORRIGIDO: scrollY ao invés de pageYOffset
            const rate = scrolled * 0.3;

            background.style.transform = 'translate3d(0, ' + rate + 'px, 0)';
            ticking = false;
        }

        window.addEventListener('scroll', () => {
            if (!ticking) {
                requestAnimationFrame(updateParallax);
                ticking = true;
            }
        }, { passive: true });
    }

    /* Animacoes ao Scroll
       ======================================================================== */
    function setupScrollAnimations() {
        const animatedElements = document.querySelectorAll('.info-card, .action-card');

        if ('IntersectionObserver' in window) {
            const animationObserver = new IntersectionObserver((entries) => {
                entries.forEach((entry, index) => {
                    if (entry.isIntersecting) {
                        // Adiciona delay sequencial aos cards
                        setTimeout(() => {
                            entry.target.style.opacity = '1';
                            entry.target.style.transform = 'translateY(0)';
                        }, index * 100);

                        animationObserver.unobserve(entry.target);
                    }
                });
            }, {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            });

            animatedElements.forEach(element => {
                // Estado inicial
                element.style.opacity = '0';
                element.style.transform = 'translateY(20px)';
                element.style.transition = 'opacity 0.6s ease, transform 0.6s ease';

                animationObserver.observe(element);
            });
        } else {
            // Fallback: mostra todos os elementos imediatamente
            animatedElements.forEach(element => {
                element.style.opacity = '1';
                element.style.transform = 'translateY(0)';
            });
        }
    }

    /* Utilitario: Smooth Scroll para Links Internos
       ======================================================================== */
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');

            // Ignora links vazios
            if (href === '#' || href === '#!') return;

            e.preventDefault();

            const target = document.querySelector(href);
            if (target) {
                const headerOffset = 100;
                const elementPosition = target.getBoundingClientRect().top;
                const offsetPosition = elementPosition + window.scrollY; // CORRIGIDO: scrollY ao invés de pageYOffset

                window.scrollTo({
                    top: offsetPosition - headerOffset, // CORRIGIDO: subtrai aqui ao invés de somar acima
                    behavior: 'smooth'
                });
            }
        });
    });

})();