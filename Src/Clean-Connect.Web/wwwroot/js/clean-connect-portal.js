(() => {
  "use strict";

  const tokenKey = "clean_connect_token";
  const userKey = "clean_connect_user";
  const apiBaseKey = "clean_connect_api_base";
  const defaultApiBase = document.querySelector('meta[name="clean-connect-api-base"]')?.content || "http://localhost:5022";

  const getApiBase = () => (localStorage.getItem(apiBaseKey) || defaultApiBase).replace(/\/$/, "");

  const decodeJwtPayload = (token) => {
    try {
      const payload = token.split(".")[1];
      return JSON.parse(atob(payload.replace(/-/g, "+").replace(/_/g, "/")));
    } catch {
      return {};
    }
  };

  const normalizeRoles = (value) => {
    if (!value) return [];
    if (Array.isArray(value)) return value;
    return [value];
  };

  const requestJson = async (endpoint, options = {}) => {
    const token = localStorage.getItem(tokenKey);
    const response = await fetch(`${getApiBase()}${endpoint}`, {
      method: options.method || "GET",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {})
      },
      body: options.body ? JSON.stringify(options.body) : undefined
    });
    const text = await response.text();
    const payload = text ? JSON.parse(text) : null;
    if (!response.ok) {
      throw new Error(payload?.message || payload?.title || response.statusText);
    }
    return payload;
  };

  const setCookie = (name, value) => {
    document.cookie = `${name}=${encodeURIComponent(value)}; path=/; max-age=604800; SameSite=Lax`;
  };

  const showAlert = (form, message, type = "success") => {
    const alert = form.querySelector("[data-form-alert]");
    if (!alert) return;
    alert.textContent = message;
    alert.className = `alert alert-${type === "error" ? "danger" : "success"}`;
    alert.hidden = false;
  };

  const validateForm = (form) => {
    let isValid = true;
    form.querySelectorAll("[required]").forEach((field) => {
      const invalid = !field.value.trim();
      field.setAttribute("aria-invalid", invalid ? "true" : "false");
      isValid = isValid && !invalid;
    });
    return isValid;
  };

  const initMenu = () => {
    const button = document.querySelector("[data-menu-toggle]");
    const menu = document.querySelector("[data-menu]");
    button?.addEventListener("click", () => menu?.classList.toggle("is-open"));
  };

  const initAuthForms = () => {
    document.querySelectorAll("[data-auth-form]").forEach((form) => {
      form.addEventListener("submit", async (event) => {
        event.preventDefault();
        if (!validateForm(form)) {
          showAlert(form, "Please complete the highlighted fields.", "error");
          return;
        }

        const button = form.querySelector("[type='submit']");
        const original = button?.textContent;
        const mode = form.dataset.authForm;
        const formData = new FormData(form);
        const role = formData.get("role") || "Client";

        if (button) {
          button.disabled = true;
          button.textContent = "Working...";
        }

        try {
          if (mode === "register") {
            await requestJson("/api/v1/Account/register", {
              method: "POST",
              body: {
                email: formData.get("email"),
                password: formData.get("password"),
                role
              }
            });
            showAlert(form, "Account created. Sign in with your new backend role.");
            window.setTimeout(() => window.location.assign("/Account/Login"), 900);
            return;
          }

          const login = await requestJson("/api/v1/Account/Login", {
            method: "POST",
            body: {
              email: formData.get("email"),
              password: formData.get("password"),
              rememberMe: formData.get("rememberMe") === "on"
            }
          });
          const claims = decodeJwtPayload(login.token);
          const roles = login.roles?.length ? login.roles : normalizeRoles(claims.role);
          const primaryRole = roles.includes("Worker") ? "Worker" : "Client";
          localStorage.setItem(tokenKey, login.token);
          localStorage.setItem(userKey, JSON.stringify({ userId: login.userId, email: login.email, roles }));
          setCookie("cc_portal_role", primaryRole);
          window.location.assign(primaryRole === "Worker" ? "/worker" : "/client");
        } catch (error) {
          showAlert(form, error.message || "The request could not be completed.", "error");
        } finally {
          if (button) {
            button.disabled = false;
            button.textContent = original;
          }
        }
      });
    });
  };

  const initDemoForms = () => {
    document.querySelectorAll("[data-demo-form]").forEach((form) => {
      form.addEventListener("submit", (event) => {
        event.preventDefault();
        if (!validateForm(form)) {
          showAlert(form, "Please complete the highlighted fields.", "error");
          return;
        }
        const success = form.dataset.success || "Saved successfully.";
        showAlert(form, success);
      });
    });
  };

  const initActionButtons = () => {
    document.querySelectorAll("[data-action-state]").forEach((button) => {
      button.addEventListener("click", () => {
        const target = document.querySelector(button.dataset.actionTarget || "");
        if (target) {
          target.textContent = button.dataset.actionState;
          target.className = `cc-status ${button.dataset.actionTone || "success"}`;
        }
      });
    });
  };

  initMenu();
  initAuthForms();
  initDemoForms();
  initActionButtons();
})();
