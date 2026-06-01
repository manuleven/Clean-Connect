(() => {
  const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const currency = new Intl.NumberFormat("en-NG", {
    style: "currency",
    currency: "NGN",
    maximumFractionDigits: 0
  });

  const getCssColor = (name) =>
    getComputedStyle(document.documentElement).getPropertyValue(name).trim();

  const escapeHtml = (value = "") =>
    String(value)
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");

  const iconSvg = (icon = "sparkle") => {
    const paths = {
      calendar: '<path d="M7 3v4M17 3v4M4 9h16M6 5h12a2 2 0 0 1 2 2v11a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V7a2 2 0 0 1 2-2Z"/><path d="M8 13h3M8 17h6"/>',
      card: '<path d="M4 7a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v10a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V7Z"/><path d="M4 10h16M7 15h4"/>',
      route: '<path d="M6 19a3 3 0 1 0 0-6 3 3 0 0 0 0 6ZM18 11a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z"/><path d="M8.5 15.5h4A3.5 3.5 0 0 0 16 12V9"/>',
      chart: '<path d="M4 19V5M4 19h16"/><path d="M8 16v-5M12 16V8M16 16v-8"/>',
      bell: '<path d="M18 9a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9Z"/><path d="M10 21h4"/>',
      shield: '<path d="M12 3 5 6v5c0 5 3 8 7 10 4-2 7-5 7-10V6l-7-3Z"/><path d="m9 12 2 2 4-5"/>',
      office: '<path d="M4 20h16M6 20V5h8v15M14 10h4v10"/><path d="M9 8h2M9 12h2M9 16h2"/>',
      user: '<path d="M12 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8Z"/><path d="M4 20a8 8 0 0 1 16 0"/>',
      check: '<path d="m5 12 4 4L19 6"/>',
      sparkle: '<path d="M12 3v6M12 15v6M3 12h6M15 12h6"/><path d="m7 7 2 2M15 15l2 2M17 7l-2 2M9 15l-2 2"/>'
    };

    return `<span class="app-icon" aria-hidden="true"><svg viewBox="0 0 24 24">${paths[icon] || paths.sparkle}</svg></span>`;
  };

  const initTheme = () => {
    const button = document.querySelector("[data-theme-toggle]");
    if (!button) return;

    const updateLabel = () => {
      const theme = document.documentElement.dataset.theme;
      button.setAttribute("aria-label", theme === "dark" ? "Switch to light mode" : "Switch to dark mode");
    };

    button.addEventListener("click", () => {
      const nextTheme = document.documentElement.dataset.theme === "dark" ? "light" : "dark";
      document.documentElement.dataset.theme = nextTheme;
      localStorage.setItem("clean-connect-theme", nextTheme);
      updateLabel();
      window.dispatchEvent(new CustomEvent("clean-connect:theme-change"));
    });

    updateLabel();
  };

  const initNavigation = () => {
    const toggle = document.querySelector("[data-nav-toggle]");
    const menu = document.querySelector("[data-nav-menu]");
    if (!toggle || !menu) return;

    const close = () => {
      menu.classList.remove("is-open");
      document.body.classList.remove("nav-open");
      toggle.setAttribute("aria-expanded", "false");
    };

    toggle.addEventListener("click", () => {
      const isOpen = menu.classList.toggle("is-open");
      document.body.classList.toggle("nav-open", isOpen);
      toggle.setAttribute("aria-expanded", String(isOpen));
    });

    menu.addEventListener("click", (event) => {
      if (event.target.closest("a")) close();
    });

    window.addEventListener("keydown", (event) => {
      if (event.key === "Escape") close();
    });
  };

  const initReveal = () => {
    const nodes = [...document.querySelectorAll(".reveal-lift")];
    if (!nodes.length || prefersReducedMotion) {
      nodes.forEach((node) => node.classList.add("is-visible"));
      return;
    }

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("is-visible");
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.18 }
    );

    nodes.forEach((node) => observer.observe(node));
  };

  const initTilt = () => {
    if (prefersReducedMotion || !window.matchMedia("(pointer: fine)").matches) return;

    document.querySelectorAll(".tilt-card").forEach((card) => {
      card.addEventListener("pointermove", (event) => {
        const rect = card.getBoundingClientRect();
        const x = (event.clientX - rect.left) / rect.width - 0.5;
        const y = (event.clientY - rect.top) / rect.height - 0.5;
        card.style.setProperty("--tilt-y", `${x * 5}deg`);
        card.style.setProperty("--tilt-x", `${y * -5}deg`);
        card.classList.add("is-tilting");
      });

      card.addEventListener("pointerleave", () => {
        card.style.removeProperty("--tilt-y");
        card.style.removeProperty("--tilt-x");
        card.classList.remove("is-tilting");
      });
    });
  };

  const initBooking = () => {
    const root = document.querySelector("[data-booking-page]");
    if (!root) return;

    const selectedService = root.querySelector("[data-selected-service]");
    const selectedDuration = root.querySelector("[data-selected-duration]");
    const selectedPrice = root.querySelector("[data-selected-price]");

    root.querySelectorAll("[data-service-option]").forEach((button) => {
      button.addEventListener("click", () => {
        root.querySelectorAll("[data-service-option]").forEach((item) => {
          item.classList.remove("is-selected");
          item.setAttribute("aria-pressed", "false");
        });

        button.classList.add("is-selected");
        button.setAttribute("aria-pressed", "true");

        if (selectedService) selectedService.textContent = button.dataset.name;
        if (selectedDuration) selectedDuration.textContent = button.dataset.duration;
        if (selectedPrice) selectedPrice.textContent = currency.format(Number(button.dataset.price || 0));
      });
    });
  };

  const initPayment = () => {
    const root = document.querySelector("[data-payment-page]");
    if (!root) return;

    root.querySelectorAll("[data-payment-method]").forEach((button) => {
      button.addEventListener("click", () => {
        root.querySelectorAll("[data-payment-method]").forEach((item) => {
          item.classList.remove("is-selected");
          item.setAttribute("aria-pressed", "false");
        });
        button.classList.add("is-selected");
        button.setAttribute("aria-pressed", "true");
      });
    });
  };

  const chartState = {
    revenue: [
      { label: "Mon", value: 230 },
      { label: "Tue", value: 268 },
      { label: "Wed", value: 248 },
      { label: "Thu", value: 292 },
      { label: "Fri", value: 326 },
      { label: "Sat", value: 360 }
    ],
    bookings: [
      { label: "Mon", value: 70 },
      { label: "Tue", value: 84 },
      { label: "Wed", value: 92 },
      { label: "Thu", value: 88 },
      { label: "Fri", value: 112 },
      { label: "Sat", value: 128 }
    ],
    mix: [
      { label: "Home", value: 42 },
      { label: "Deep", value: 31 },
      { label: "Office", value: 18 },
      { label: "Move-in", value: 9 }
    ]
  };

  const setupCanvas = (canvas) => {
    const rect = canvas.getBoundingClientRect();
    const ratio = Math.max(window.devicePixelRatio || 1, 1);
    const width = Math.max(rect.width, 320);
    const height = Math.max(rect.height || Number(canvas.getAttribute("height")) || 280, 260);
    canvas.width = Math.floor(width * ratio);
    canvas.height = Math.floor(height * ratio);
    const context = canvas.getContext("2d");
    context.setTransform(ratio, 0, 0, ratio, 0, 0);
    return { context, width, height };
  };

  const drawAxes = (context, width, height, padding) => {
    context.strokeStyle = getCssColor("--line");
    context.lineWidth = 1;
    context.beginPath();
    context.moveTo(padding, padding);
    context.lineTo(padding, height - padding);
    context.lineTo(width - padding, height - padding);
    context.stroke();
  };

  const drawLineChart = (canvas, points) => {
    const { context, width, height } = setupCanvas(canvas);
    const padding = 34;
    const values = points.map((point) => Number(point.value));
    const min = Math.min(...values) * 0.86;
    const max = Math.max(...values) * 1.08;
    const range = Math.max(max - min, 1);
    const accent = getCssColor("--mint");
    const sky = getCssColor("--sky");

    context.clearRect(0, 0, width, height);
    drawAxes(context, width, height, padding);

    const coordinates = points.map((point, index) => {
      const x = padding + (index / Math.max(points.length - 1, 1)) * (width - padding * 2);
      const y = height - padding - ((Number(point.value) - min) / range) * (height - padding * 2);
      return { x, y, label: point.label };
    });

    const gradient = context.createLinearGradient(padding, 0, width - padding, 0);
    gradient.addColorStop(0, accent);
    gradient.addColorStop(1, sky);

    context.strokeStyle = gradient;
    context.lineWidth = 4;
    context.lineJoin = "round";
    context.beginPath();
    coordinates.forEach((point, index) => {
      if (index === 0) context.moveTo(point.x, point.y);
      else context.lineTo(point.x, point.y);
    });
    context.stroke();

    context.fillStyle = accent;
    coordinates.forEach((point) => {
      context.beginPath();
      context.arc(point.x, point.y, 5, 0, Math.PI * 2);
      context.fill();
    });

    context.fillStyle = getCssColor("--muted");
    context.font = "700 12px Inter, system-ui, sans-serif";
    coordinates.forEach((point) => {
      context.fillText(point.label, point.x - 12, height - 10);
    });
  };

  const drawBarChart = (canvas, points) => {
    const { context, width, height } = setupCanvas(canvas);
    const padding = 34;
    const values = points.map((point) => Number(point.value));
    const max = Math.max(...values) * 1.15;
    const barGap = 14;
    const barWidth = (width - padding * 2 - barGap * (points.length - 1)) / points.length;

    context.clearRect(0, 0, width, height);
    drawAxes(context, width, height, padding);

    points.forEach((point, index) => {
      const value = Number(point.value);
      const x = padding + index * (barWidth + barGap);
      const barHeight = (value / max) * (height - padding * 2);
      const y = height - padding - barHeight;
      const gradient = context.createLinearGradient(0, y, 0, height - padding);
      gradient.addColorStop(0, getCssColor("--sky"));
      gradient.addColorStop(1, getCssColor("--mint"));
      context.fillStyle = gradient;
      roundRect(context, x, y, barWidth, barHeight, 8);
      context.fill();
      context.fillStyle = getCssColor("--muted");
      context.font = "700 12px Inter, system-ui, sans-serif";
      context.fillText(point.label, x + Math.max(barWidth / 2 - 12, 0), height - 10);
    });
  };

  const drawDonutChart = (canvas, points) => {
    const { context, width, height } = setupCanvas(canvas);
    const total = points.reduce((sum, point) => sum + Number(point.value), 0) || 1;
    const radius = Math.min(width, height) / 2 - 42;
    const centerX = width / 2;
    const centerY = height / 2 - 8;
    const colors = [getCssColor("--mint"), getCssColor("--sky"), getCssColor("--amber"), getCssColor("--rose")];
    let start = -Math.PI / 2;

    context.clearRect(0, 0, width, height);
    points.forEach((point, index) => {
      const slice = (Number(point.value) / total) * Math.PI * 2;
      context.beginPath();
      context.arc(centerX, centerY, radius, start, start + slice);
      context.lineWidth = 24;
      context.strokeStyle = colors[index % colors.length];
      context.stroke();
      start += slice;
    });

    context.fillStyle = getCssColor("--ink");
    context.font = "900 24px Inter, system-ui, sans-serif";
    context.textAlign = "center";
    context.fillText(`${Math.round(total)}`, centerX, centerY + 6);
    context.font = "700 12px Inter, system-ui, sans-serif";
    context.fillStyle = getCssColor("--muted");
    context.fillText("live jobs", centerX, centerY + 27);
    context.textAlign = "left";
  };

  const roundRect = (context, x, y, width, height, radius) => {
    const r = Math.min(radius, width / 2, height / 2);
    context.beginPath();
    context.moveTo(x + r, y);
    context.arcTo(x + width, y, x + width, y + height, r);
    context.arcTo(x + width, y + height, x, y + height, r);
    context.arcTo(x, y + height, x, y, r);
    context.arcTo(x, y, x + width, y, r);
    context.closePath();
  };

  const renderCharts = () => {
    const revenue = document.querySelector('[data-chart="revenue"]');
    const bookings = document.querySelector('[data-chart="bookings"]');
    const mix = document.querySelector('[data-chart="mix"]');
    if (revenue) drawLineChart(revenue, chartState.revenue);
    if (bookings) drawBarChart(bookings, chartState.bookings);
    if (mix) drawDonutChart(mix, chartState.mix);
  };

  const setConnectionState = (state, label) => {
    const status = document.getElementById("adminConnectionStatus");
    if (!status) return;
    status.classList.toggle("is-connected", state === "connected");
    status.classList.toggle("is-fallback", state === "fallback");
    const labelNode = status.querySelector("span:last-child");
    if (labelNode) labelNode.textContent = label;
  };

  const updateMetrics = (metrics = []) => {
    metrics.forEach((metric) => {
      document.querySelectorAll("[data-dashboard-metric]").forEach((card) => {
        if (card.dataset.dashboardMetric !== metric.label) return;
        const value = card.querySelector("[data-dashboard-metric-value]");
        const change = card.querySelector("[data-dashboard-metric-change]");
        if (value) value.textContent = metric.value;
        if (change) change.textContent = metric.change;
      });
    });
  };

  const prependActivity = (activity) => {
    const feed = document.getElementById("liveActivityFeed");
    if (!feed || !activity) return;

    const tone = escapeHtml(activity.tone || "mint");
    const icon = escapeHtml(activity.icon || "sparkle");
    const item = document.createElement("article");
    item.className = "activity-item";
    item.innerHTML = `
      <span class="icon-bubble icon-bubble--${tone}">${iconSvg(icon)}</span>
      <span>
        <strong>${escapeHtml(activity.title)}</strong>
        <small>${escapeHtml(activity.description)}</small>
      </span>
      <time>${escapeHtml(activity.time)}</time>
    `;
    feed.prepend(item);
    while (feed.children.length > 6) feed.lastElementChild.remove();
  };

  const prependNotification = (notification) => {
    const feed = document.getElementById("liveNotifications");
    if (!feed || !notification) return;

    const tone = escapeHtml(notification.tone || "mint");
    const item = document.createElement("article");
    item.className = `notification-item notification-item--${tone}`;
    item.innerHTML = `
      <strong>${escapeHtml(notification.title)}</strong>
      <span>${escapeHtml(notification.message)}</span>
      <time>${escapeHtml(notification.time)}</time>
    `;
    feed.prepend(item);
    while (feed.children.length > 6) feed.lastElementChild.remove();
  };

  const applyDashboardSnapshot = (snapshot) => {
    if (!snapshot) return;
    updateMetrics(snapshot.metrics || []);
    if (snapshot.revenue?.length) chartState.revenue = snapshot.revenue;
    if (snapshot.bookings?.length) chartState.bookings = snapshot.bookings;
    if (snapshot.serviceMix?.length) chartState.mix = snapshot.serviceMix;
    prependActivity(snapshot.activity);
    prependNotification(snapshot.notification);
    renderCharts();
  };

  const startFallbackDashboard = () => {
    setConnectionState("fallback", "Demo live feed active");
    let tick = 0;
    window.setInterval(() => {
      tick += 1;
      const metrics = [
        { label: "Revenue today", value: `₦${(2.7 + tick * 0.03).toFixed(2)}m`, change: "+21%" },
        { label: "Active bookings", value: String(180 + (tick % 16)), change: `+${tick % 8}` },
        { label: "Pending payouts", value: `₦${640 - tick * 3}k`, change: "-8%" },
        { label: "Support SLA", value: `${94 + (tick % 4)}%`, change: "+4%" }
      ];

      chartState.revenue = chartState.revenue.map((point, index) => ({
        ...point,
        value: 250 + index * 22 + Math.round(Math.sin((tick + index) / 2) * 22)
      }));
      chartState.bookings = chartState.bookings.map((point, index) => ({
        ...point,
        value: 78 + index * 9 + Math.round(Math.cos((tick + index) / 2) * 10)
      }));
      chartState.mix = chartState.mix.map((point, index) => ({
        ...point,
        value: 18 + ((tick + index * 11) % 36)
      }));

      applyDashboardSnapshot({
        metrics,
        revenue: chartState.revenue,
        bookings: chartState.bookings,
        serviceMix: chartState.mix,
        activity: {
          title: "Demo update",
          description: "Live dashboard fallback generated a fresh operations event.",
          time: "just now",
          tone: tick % 2 ? "mint" : "sky",
          icon: tick % 2 ? "route" : "card"
        },
        notification: {
          title: "Fallback mode",
          message: "SignalR client library was unavailable, so local updates are running.",
          time: "live",
          tone: "amber"
        }
      });
    }, 5000);
  };

  const connectNativeSignalR = async () => {
    const endpoint = "/hubs/admin-dashboard";
    const negotiate = await fetch(`${endpoint}/negotiate?negotiateVersion=1`, {
      method: "POST",
      headers: { "X-Requested-With": "XMLHttpRequest" }
    });

    if (!negotiate.ok) {
      throw new Error("SignalR negotiate failed");
    }

    const payload = await negotiate.json();
    const token = encodeURIComponent(payload.connectionToken || payload.connectionId);
    const protocol = window.location.protocol === "https:" ? "wss:" : "ws:";
    const socket = new WebSocket(`${protocol}//${window.location.host}${endpoint}?id=${token}`);
    const recordSeparator = String.fromCharCode(30);

    socket.addEventListener("open", () => {
      socket.send(`${JSON.stringify({ protocol: "json", version: 1 })}${recordSeparator}`);
      setConnectionState("connected", "SignalR live feed");
    });

    socket.addEventListener("message", (event) => {
      String(event.data)
        .split(recordSeparator)
        .filter(Boolean)
        .forEach((message) => {
          const packet = JSON.parse(message);
          if (packet.type === 1 && packet.target === "dashboard:update") {
            applyDashboardSnapshot(packet.arguments?.[0]);
          }
        });
    });

    socket.addEventListener("close", () => {
      setConnectionState("fallback", "Reconnecting live feed");
      window.setTimeout(() => {
        connectNativeSignalR().catch(startFallbackDashboard);
      }, 2500);
    });

    socket.addEventListener("error", () => {
      socket.close();
    });
  };

  const initDashboard = async () => {
    if (!document.querySelector("[data-admin-dashboard]")) return;

    renderCharts();
    window.addEventListener("resize", renderCharts);
    window.addEventListener("clean-connect:theme-change", renderCharts);

    try {
      await connectNativeSignalR();
    } catch {
      startFallbackDashboard();
    }
  };

  document.addEventListener("DOMContentLoaded", () => {
    initTheme();
    initNavigation();
    initReveal();
    initTilt();
    initBooking();
    initPayment();
    initDashboard();
  });
})();
