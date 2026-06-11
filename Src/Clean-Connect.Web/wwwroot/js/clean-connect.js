(() => {
  "use strict";

  const apiBaseMeta = document.querySelector('meta[name="clean-connect-api-base"]');
  const defaultApiBase = apiBaseMeta?.content || "http://localhost:5022";
  const tokenKey = "clean_connect_token";
  const emailKey = "clean_connect_user_email";
  const userIdKey = "clean_connect_user_id";
  const clientKey = "clean_connect_client";
  const apiBaseKey = "clean_connect_api_base";

  const getApiBase = () => (localStorage.getItem(apiBaseKey) || defaultApiBase).replace(/\/$/, "");
  const token = () => localStorage.getItem(tokenKey) || "";

  const prettyJson = (value) => {
    if (typeof value === "string") return value;
    return JSON.stringify(value, null, 2);
  };

  const normalizeEndpoint = (endpoint) => {
    if (/^https?:\/\//i.test(endpoint)) return endpoint;
    return `${getApiBase()}${endpoint.startsWith("/") ? endpoint : `/${endpoint}`}`;
  };

  const tryParseJson = async (response) => {
    const text = await response.text();
    if (!text) return null;
    try {
      return JSON.parse(text);
    } catch {
      return text;
    }
  };

  const showResponse = (panel, value, ok = true) => {
    if (!panel) return;
    panel.textContent = prettyJson(value);
    panel.classList.add("is-visible");
    panel.classList.toggle("is-success", ok);
    panel.classList.toggle("is-error", !ok);
  };

  const castValue = (input) => {
    if (input.type === "checkbox") return input.checked;
    if (input.dataset.type === "number" || input.type === "number" || input.type === "range") {
      return input.value === "" ? null : Number(input.value);
    }
    if (input.dataset.type === "date") {
      return input.value ? new Date(input.value).toISOString() : null;
    }
    if (input.dataset.type === "guid-null" && !input.value) return null;
    return input.value;
  };

  const collectFormData = (form) => {
    const data = {};
    const pathFields = new Set();
    form.querySelectorAll("[name]").forEach((input) => {
      if (input.disabled) return;
      if ((input.type === "radio" || input.type === "checkbox") && !input.checked && input.dataset.type !== "boolean") return;
      data[input.name] = castValue(input);
      if (input.dataset.pathField === "true") pathFields.add(input.name);
    });
    return { data, pathFields };
  };

  const interpolateEndpoint = (endpoint, data, pathFields) =>
    endpoint.replace(/\{([^}]+)\}/g, (_, key) => {
      pathFields.add(key);
      return encodeURIComponent(data[key] ?? "");
    });

  const requestJson = async (endpoint, options = {}) => {
    const headers = {
      Accept: "application/json",
      ...(options.body ? { "Content-Type": "application/json" } : {}),
      ...(options.auth && token() ? { Authorization: `Bearer ${token()}` } : {})
    };

    const response = await fetch(normalizeEndpoint(endpoint), {
      method: options.method || "GET",
      headers,
      body: options.body ? JSON.stringify(options.body) : undefined
    });
    const payload = await tryParseJson(response);
    if (!response.ok) {
      const detail = typeof payload === "string" ? payload : payload?.message || payload?.title || response.statusText;
      const error = new Error(detail || "Request failed");
      error.payload = payload;
      error.status = response.status;
      throw error;
    }
    return payload;
  };

  const initApiForms = () => {
    document.querySelectorAll("[data-api-form]").forEach((form) => {
      form.addEventListener("submit", async (event) => {
        event.preventDefault();
        const card = form.closest(".cc-api-card, .card") || document;
        const panel = card.querySelector("[data-api-response]");
        const button = form.querySelector("[type='submit']");
        const originalText = button?.textContent;
        const method = (form.dataset.method || "POST").toUpperCase();
        const auth = form.dataset.auth === "true";
        const { data, pathFields } = collectFormData(form);
        const endpoint = interpolateEndpoint(form.dataset.endpoint || "", data, pathFields);
        pathFields.forEach((key) => delete data[key]);

        if (button) {
          button.disabled = true;
          button.textContent = "Working...";
        }

        try {
          let result;
          if (method === "GET") {
            const query = new URLSearchParams();
            Object.entries(data).forEach(([key, value]) => {
              if (value !== null && value !== "") query.set(key, value);
            });
            result = await requestJson(`${endpoint}${query.toString() ? `?${query}` : ""}`, { method, auth });
          } else {
            result = await requestJson(endpoint, { method, auth, body: data });
          }

          showResponse(panel, result ?? { success: true }, true);
          document.dispatchEvent(new CustomEvent("clean-connect:api-success", { detail: { form, result } }));
        } catch (error) {
          showResponse(panel, error.payload || { status: error.status, message: error.message }, false);
        } finally {
          if (button) {
            button.disabled = false;
            button.textContent = originalText;
          }
        }
      });
    });
  };

  const initApiBaseControls = () => {
    document.querySelectorAll("[data-api-base-input]").forEach((input) => {
      input.value = getApiBase();
      input.addEventListener("change", () => {
        localStorage.setItem(apiBaseKey, input.value.replace(/\/$/, ""));
      });
    });
  };

  const initSelectableCards = () => {
    document.querySelectorAll("[data-select-group]").forEach((group) => {
      group.querySelectorAll("[data-select-card]").forEach((button) => {
        button.addEventListener("click", () => {
          group.querySelectorAll("[data-select-card]").forEach((item) => {
            item.classList.remove("is-selected");
            item.setAttribute("aria-pressed", "false");
          });
          button.classList.add("is-selected");
          button.setAttribute("aria-pressed", "true");

          Object.entries(button.dataset).forEach(([key, value]) => {
            if (!key.startsWith("summary")) return;
            const target = document.querySelector(`[data-summary="${key.replace("summary", "").toLowerCase()}"]`);
            if (target) target.textContent = value;
          });
        });
      });
    });
  };

  const initFilters = () => {
    document.querySelectorAll("[data-table-filter]").forEach((input) => {
      const target = document.querySelector(input.dataset.tableFilter);
      if (!target) return;
      input.addEventListener("input", () => {
        const value = input.value.trim().toLowerCase();
        target.querySelectorAll("tbody tr, [data-filter-row]").forEach((row) => {
          row.classList.toggle("d-none", value && !row.textContent.toLowerCase().includes(value));
        });
      });
    });
  };

  const initLocalLogout = () => {
    document.querySelectorAll("[data-local-logout]").forEach((link) => {
      link.addEventListener("click", () => {
        localStorage.removeItem(tokenKey);
        localStorage.removeItem(emailKey);
        localStorage.removeItem(userIdKey);
        localStorage.removeItem(clientKey);
      });
    });
  };

  const initThemeButton = () => {
    const button = document.querySelector("[data-theme-toggle]");
    if (!button) return;
    const render = () => {
      const isDark = document.documentElement.getAttribute("data-theme") === "dark";
      button.innerHTML = `<iconify-icon icon="${isDark ? "solar:sun-2-outline" : "solar:moon-stars-outline"}"></iconify-icon>`;
      button.style.fontSize = "0";
    };
    window.setTimeout(render, 0);
    button.addEventListener("click", () => window.setTimeout(render, 0));
  };

  const initCharts = () => {
    if (!window.ApexCharts) return;

    document.querySelectorAll("[data-cc-chart]").forEach((element) => {
      const kind = element.dataset.ccChart;
      const palette = ["#2563eb", "#14b8a6", "#f59e0b", "#e11d48"];
      const common = {
        chart: { toolbar: { show: false }, parentHeightOffset: 0 },
        colors: palette,
        grid: { borderColor: "#e5e7eb" },
        dataLabels: { enabled: false },
        stroke: { curve: "smooth", width: 3 }
      };
      const configs = {
        revenue: {
          ...common,
          chart: { ...common.chart, type: "area", height: 280 },
          series: [{ name: "Revenue", data: [2.1, 2.4, 2.2, 2.7, 3.1, 2.84, 3.4] }],
          xaxis: { categories: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"] },
          yaxis: { labels: { formatter: (value) => `NGN ${value.toFixed(1)}m` } },
          fill: { type: "gradient", gradient: { opacityFrom: 0.28, opacityTo: 0.02 } }
        },
        bookings: {
          ...common,
          chart: { ...common.chart, type: "bar", height: 280 },
          plotOptions: { bar: { borderRadius: 8, columnWidth: "48%" } },
          series: [{ name: "Bookings", data: [72, 88, 96, 104, 121, 136, 118] }],
          xaxis: { categories: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"] }
        },
        mix: {
          ...common,
          chart: { ...common.chart, type: "donut", height: 280 },
          labels: ["Home", "Deep", "Office", "Move-in"],
          series: [42, 31, 18, 9],
          legend: { position: "bottom" }
        },
        wallet: {
          ...common,
          chart: { ...common.chart, type: "line", height: 260 },
          series: [
            { name: "Held", data: [1.8, 2.1, 2.6, 2.4, 3.1, 3.4] },
            { name: "Released", data: [1.2, 1.6, 1.8, 2.2, 2.6, 2.9] }
          ],
          xaxis: { categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun"] }
        }
      };
      if (configs[kind]) {
        const chart = new ApexCharts(element, configs[kind]);
        chart.render();
        element._ccChart = chart;
      }
    });
  };

  const prependFeedItem = (selector, item, kind = "activity") => {
    const feed = document.querySelector(selector);
    if (!feed) return;
    const node = document.createElement("div");
    node.className = kind === "notification"
      ? `p-16 radius-8 bg-${item.tone || "primary"}-50`
      : "cc-action-item";
    node.innerHTML = kind === "notification"
      ? `<strong class="d-block text-primary-light">${item.title}</strong><span class="text-sm text-secondary-light">${item.message}</span><time class="text-xs text-secondary-light d-block mt-4">${item.time}</time>`
      : `<span class="cc-stat-icon cc-stat-icon--${item.tone || "mint"}"><iconify-icon icon="${item.icon || "solar:bell-outline"}"></iconify-icon></span><span><strong class="d-block">${item.title}</strong><small class="text-secondary-light">${item.description}</small></span><time class="text-sm text-secondary-light">${item.time}</time>`;
    feed.prepend(node);
    while (feed.children.length > 6) feed.lastElementChild.remove();
  };

  const updateDashboardMetrics = (metrics = []) => {
    metrics.forEach((metric) => {
      document.querySelectorAll(`[data-dashboard-metric="${metric.label}"]`).forEach((card) => {
        const value = card.querySelector("[data-dashboard-metric-value]");
        const change = card.querySelector("[data-dashboard-metric-change]");
        if (value) value.textContent = metric.value;
        if (change) change.textContent = metric.change;
      });
    });
  };

  const startAdminFallback = () => {
    const status = document.getElementById("adminConnectionStatus");
    if (status) status.textContent = "Demo live feed";
    let tick = 0;
    window.setInterval(() => {
      tick += 1;
      updateDashboardMetrics([
        { label: "Revenue today", value: `NGN ${(2.84 + tick * 0.02).toFixed(2)}m`, change: "+24%" },
        { label: "Active bookings", value: String(186 + (tick % 9)), change: `+${tick % 7}` },
        { label: "Pending payouts", value: `NGN ${640 - tick * 2}k`, change: "-8%" },
        { label: "Support SLA", value: `${96 + (tick % 3)}%`, change: "+4%" }
      ]);
      prependFeedItem("#liveActivityFeed", {
        title: "Live ops event",
        description: "A fresh booking, payment, or dispatch event was simulated for the dashboard.",
        time: "just now",
        tone: tick % 2 ? "mint" : "sky",
        icon: tick % 2 ? "solar:map-point-wave-outline" : "solar:card-outline"
      });
      prependFeedItem("#liveNotifications", {
        title: "Operations pulse",
        message: "Dashboard cards and feeds are still updating while SignalR is unavailable.",
        time: "live",
        tone: "warning"
      }, "notification");
    }, 5000);
  };

  const connectAdminHub = async () => {
    if (!document.querySelector("[data-admin-dashboard]")) return;
    const status = document.getElementById("adminConnectionStatus");
    try {
      const endpoint = "/hubs/admin-dashboard";
      const negotiate = await fetch(`${endpoint}/negotiate?negotiateVersion=1`, { method: "POST" });
      if (!negotiate.ok) throw new Error("SignalR negotiate failed");
      const payload = await negotiate.json();
      const tokenValue = encodeURIComponent(payload.connectionToken || payload.connectionId);
      const protocol = window.location.protocol === "https:" ? "wss:" : "ws:";
      const socket = new WebSocket(`${protocol}//${window.location.host}${endpoint}?id=${tokenValue}`);
      const recordSeparator = String.fromCharCode(30);

      socket.addEventListener("open", () => {
        socket.send(`${JSON.stringify({ protocol: "json", version: 1 })}${recordSeparator}`);
        if (status) status.textContent = "SignalR live";
      });
      socket.addEventListener("message", (event) => {
        String(event.data).split(recordSeparator).filter(Boolean).forEach((message) => {
          const packet = JSON.parse(message);
          if (packet.type !== 1 || packet.target !== "dashboard:update") return;
          const snapshot = packet.arguments?.[0];
          updateDashboardMetrics(snapshot?.metrics || []);
          prependFeedItem("#liveActivityFeed", snapshot?.activity || {}, "activity");
          prependFeedItem("#liveNotifications", snapshot?.notification || {}, "notification");
        });
      });
      socket.addEventListener("close", startAdminFallback, { once: true });
      socket.addEventListener("error", () => socket.close());
    } catch {
      startAdminFallback();
    }
  };

  const initLogin = () => {
    const form = document.getElementById("cc-login-form");
    if (!form) return;
    form.addEventListener("submit", async (event) => {
      event.preventDefault();
      const panel = document.querySelector("[data-login-response]");
      const { data } = collectFormData(form);
      try {
        const result = await requestJson("/api/v1/Account/Login", { method: "POST", body: data });
        if (result?.token) {
          localStorage.setItem(tokenKey, result.token);
          localStorage.setItem(emailKey, data.email);
          localStorage.setItem(userIdKey, result.userId || "");
        }
        showResponse(panel, { success: true, message: "Signed in. Redirecting to profile.", userId: result?.userId }, true);
        window.setTimeout(() => { window.location.href = "/Account/Profile"; }, 700);
      } catch (error) {
        showResponse(panel, error.payload || { message: error.message }, false);
      }
    });
  };

  const initRegister = () => {
    const form = document.getElementById("cc-register-form");
    if (!form) return;
    form.addEventListener("submit", async (event) => {
      event.preventDefault();
      const panel = document.querySelector("[data-register-response]");
      const { data } = collectFormData(form);
      const identity = { email: data.email, password: data.password };
      delete data.password;
      try {
        const clientResult = await requestJson("/api/Client/register-client", { method: "POST", body: data });
        const identityResult = await requestJson("/api/v1/Account/register", { method: "POST", body: identity });
        localStorage.setItem(emailKey, identity.email);
        showResponse(panel, {
          success: true,
          clientCreated: clientResult,
          userId: identityResult,
          next: "Confirm email if required, then sign in."
        }, true);
      } catch (error) {
        showResponse(panel, error.payload || { message: error.message }, false);
      }
    });
  };

  const initProfile = async () => {
    const root = document.querySelector("[data-profile-page]");
    if (!root) return;
    const signedOut = root.querySelector("[data-profile-signed-out]");
    const signedIn = root.querySelector("[data-profile-signed-in]");
    const email = localStorage.getItem(emailKey);
    if (!token() || !email) {
      signedOut?.classList.remove("d-none");
      signedIn?.classList.add("d-none");
      return;
    }
    signedOut?.classList.add("d-none");
    signedIn?.classList.remove("d-none");

    const cached = localStorage.getItem(clientKey);
    if (cached) renderProfile(JSON.parse(cached));
    try {
      const clients = await requestJson("/api/Client/all-clients", { method: "GET", auth: true });
      const client = Array.isArray(clients)
        ? clients.find((item) => (item.email?.value || item.email || "").toLowerCase() === email.toLowerCase())
        : null;
      if (client) {
        localStorage.setItem(clientKey, JSON.stringify(client));
        renderProfile(client);
      }
    } catch {
      root.querySelector("[data-profile-email]").textContent = email;
    }
  };

  const renderProfile = (client) => {
    const firstName = client.fullName?.firstName || client.firstName || "Clean";
    const lastName = client.fullName?.lastName || client.lastName || "Client";
    const email = client.email?.value || client.email || localStorage.getItem(emailKey) || "";
    const contact = client.phoneNumber?.value || client.contact || "Not set";
    const state = client.state || "Not set";
    const referral = client.referralCode || "CLEAN-CONNECT";
    const count = Number(client.successfulReferralCount || 0);
    const percent = Math.min(100, count * 10);

    document.querySelectorAll("[data-profile-name]").forEach((node) => { node.textContent = `${firstName} ${lastName}`; });
    document.querySelectorAll("[data-profile-avatar]").forEach((node) => { node.textContent = `${firstName[0] || "C"}${lastName[0] || "C"}`.toUpperCase(); });
    document.querySelectorAll("[data-profile-email]").forEach((node) => { node.textContent = email; });
    document.querySelectorAll("[data-profile-contact]").forEach((node) => { node.textContent = contact; });
    document.querySelectorAll("[data-profile-state]").forEach((node) => { node.textContent = state; });
    document.querySelectorAll("[data-profile-referral]").forEach((node) => { node.textContent = referral; });
    document.querySelectorAll("[data-profile-referral-count]").forEach((node) => { node.textContent = `${count} / 10 referrals`; });
    document.querySelectorAll("[data-profile-referral-progress]").forEach((node) => { node.style.width = `${percent}%`; });
  };

  const initCopyButtons = () => {
    document.querySelectorAll("[data-copy-target]").forEach((button) => {
      button.addEventListener("click", async () => {
        const target = document.querySelector(button.dataset.copyTarget);
        if (!target) return;
        await navigator.clipboard.writeText(target.textContent.trim());
        const original = button.textContent;
        button.textContent = "Copied";
        window.setTimeout(() => { button.textContent = original; }, 1300);
      });
    });
  };

  document.addEventListener("DOMContentLoaded", () => {
    initThemeButton();
    initApiBaseControls();
    initApiForms();
    initSelectableCards();
    initFilters();
    initLocalLogout();
    initCharts();
    connectAdminHub();
    initLogin();
    initRegister();
    initProfile();
    initCopyButtons();
  });
})();
