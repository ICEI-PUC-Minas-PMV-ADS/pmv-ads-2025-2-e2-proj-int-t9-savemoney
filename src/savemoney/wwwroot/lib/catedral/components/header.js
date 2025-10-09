import { CatedralComponent } from "../core/core.js";

export default class CatedralHeader extends CatedralComponent {
  static get observedAttributes() {
    return ["title"];
  }

  constructor() {
    super();
    this.menuOpen = false;
    this.toggleMenu = this.toggleMenu.bind(this);
    this.handleOutsideClick = this.handleOutsideClick.bind(this);
    console.log("[CatedralHeader] constructor");
  }

  connectedCallback() {
    console.log("[CatedralHeader] connectedCallback (antes do super)");
    super.connectedCallback();
    console.log("[CatedralHeader] connectedCallback (depois do super)");
    document.addEventListener("click", this.handleOutsideClick);
  }

  disconnectedCallback() {
    console.log("[CatedralHeader] disconnectedCallback");
    // Não precisa remover do botão aqui, pois ele é recriado a cada render
    document.removeEventListener("click", this.handleOutsideClick);
  }

  toggleMenu(e) {
    console.log("[CatedralHeader] toggleMenu", { menuOpen: this.menuOpen });
    if (e) e.stopPropagation();
    this.menuOpen = !this.menuOpen;
    this.render();
  }

  handleOutsideClick(e) {
    const path = e.composedPath ? e.composedPath() : [];
    const isInside = path.includes(this);
    // Também verifica se clicou no botão ou no dropdown
    const btn = this.shadowRoot?.querySelector('.menu-btn');
    const dropdown = this.shadowRoot?.querySelector('.dropdown');
    const clickedBtn = btn && path.includes(btn);
    const clickedDropdown = dropdown && path.includes(dropdown);

    console.log("[CatedralHeader] handleOutsideClick", {
      target: e.target,
      menuOpen: this.menuOpen,
      isInside,
      clickedBtn,
      clickedDropdown
    });

    // Só fecha se não clicou no header, nem no botão, nem no dropdown
    if (!isInside && !clickedBtn && !clickedDropdown && this.menuOpen) {
      this.menuOpen = false;
      this.render();
    }
  }

  async afterRender() {
    // Sempre adiciona o event listener ao botão após renderização
    const btn = this.shadowRoot && this.shadowRoot.querySelector('.menu-btn');
    if (btn) {
      btn.removeEventListener('click', this.toggleMenu); // evita múltiplos listeners
      btn.addEventListener('click', this.toggleMenu);
    }

    // Adiciona o event listener ao overlay para fechar o menu ao clicar
    const overlay = this.shadowRoot && this.shadowRoot.querySelector('.dropdown-overlay');
    if (overlay) {
      overlay.removeEventListener('click', this._closeMenuOnOverlayClick);
      this._closeMenuOnOverlayClick = (e) => {
        e.stopPropagation();
        if (this.menuOpen) {
          this.menuOpen = false;
          this.render();
        }
      };
      overlay.addEventListener('click', this._closeMenuOnOverlayClick);
    }

    // Loga o conteúdo real do slot após renderização
    const slot = this.shadowRoot && this.shadowRoot.querySelector('slot');
    if (slot) {
      const nodes = slot.assignedNodes({flatten: true});
      console.log("[CatedralHeader] afterRender slot nodes:", nodes);
    } else {
      console.log("[CatedralHeader] afterRender: slot not found");
    }
  }

  render() {
    console.log("[CatedralHeader] render()");
    super.render && super.render();
  }

  template() {
    const title = this.getAttribute("title") || "Título";
    const menuOpen = this.menuOpen;
    return `
      <style>
        .container-header {
          position: sticky;
          top: 1rem;
          z-index: 100;
          display: flex;
          justify-content: center;
          align-items: center;
          max-width: 360px;
          margin: 0 auto;
        }
        .header {
          display: flex;
          align-items: center;
          justify-content: space-between;
          background: #eee;
          padding: 0.5rem 1rem;
          border-radius: 2rem;
          font-family: inherit;
          position: relative;
          margin: .5rem;
          position: sticky;
          top: 1rem;
          z-index: 110;
          width: 100%;
          box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }
        .header-title {
          font-size: 1.5rem;
          font-weight: 500;
          text-align: center;
          min-width: 120px;
        }
        .menu-btn {
          background: none;
          border: none;
          font-size: 1.5rem;
          cursor: pointer;
          margin-right: 1rem;
          display: flex;
          align-items: center;
          justify-content: center;
        }
        ::slotted([slot="profile"]) {
          background: none;
          border: none;
          font-size: 1.5rem;
          cursor: pointer;
          margin-left: 1rem;
          display: flex;
          align-items: center;
          justify-content: center;
        }
        .dropdown {
          position: absolute;
          top: 100%;
          left: 0;
          z-index: 110;
          min-width: 220px;
          background: #eee;
          border-radius: 1rem;
          box-shadow: 0 2px 8px rgba(0,0,0,0.08);
          margin-top: 0.5rem;
          padding: 0.5rem;
          opacity: 0;
          transform: translateY(-10px);
          pointer-events: none;
          transition: opacity 0.25s cubic-bezier(.4,0,.2,1), transform 0.25s cubic-bezier(.4,0,.2,1);
        }
        .dropdown.active {
          opacity: 1;
          transform: translateY(0);
          pointer-events: auto;
        }
        .dropdown slot {
          display: block;
        }
        .dropdown-overlay {
          position: fixed;
          inset: 0;
          width: 100vw;
          height: 100vh;
          z-index: 105;
          background: rgba(0,0,0,0.5);
          backdrop-filter: blur(4px) grayscale(0.7);
          opacity: 0;
          pointer-events: none;
          transition: opacity 0.3s cubic-bezier(.4,0,.2,1);
        }
        .dropdown-overlay.active {
          opacity: 1;
          pointer-events: auto;
        }
      </style>
      <div class="container-header">
        <div class="header">
          <button
            class="menu-btn"
            aria-label="${menuOpen ? "Fechar menu" : "Abrir menu"}"
            aria-haspopup="true"
            aria-expanded="${menuOpen ? "true" : "false"}"
            aria-controls="catedral-header-dropdown"
            type="button"
          >
            ${menuOpen
              ? `<svg width="28" height="28" viewBox="0 0 24 24" aria-hidden="true" focusable="false"><line x1="18" y1="6" x2="6" y2="18" stroke="#333" stroke-width="2"/><line x1="6" y1="6" x2="18" y2="18" stroke="#333" stroke-width="2"/></svg>`
              : `<svg width="28" height="28" viewBox="0 0 24 24" aria-hidden="true" focusable="false"><rect y="4" width="24" height="2" rx="1" fill="#333"/><rect y="11" width="24" height="2" rx="1" fill="#333"/><rect y="18" width="24" height="2" rx="1" fill="#333"/></svg>`
            }
          </button>
          <span class="header-title">${title}</span>
          <slot name="profile"></slot>
        </div>
        <div class="dropdown${menuOpen ? ' active' : ''}" id="catedral-header-dropdown">
          <slot>Menu padrão (slot vazio)</slot>
        </div>
        <div class="dropdown-overlay${menuOpen ? ' active' : ''}"></div>
      </div>
    `;
  }
}

customElements.define("catedral-header", CatedralHeader);