// ========================================
// SCHEDULE ENGINE - Pure Calculation Logic
// ========================================

const ScheduleEngine = {
  CONFIG: {
    timeIncrementMinutes: 30,
    startOfSchedule: 6 * 60,
    wakeUpTimeMin: 6 * 60 + 30,
    wakeUpTimeMax: 8 * 60 + 30,
    bedTimeMin: 20 * 60,
    bedTimeMax: 20 * 60,
    endOfSchedule: 20 * 60 + 30,
    napTimeMinutesMin: 60,
    napTimeMinutesMax: 120,
    awakeTimeMinutesMin: 60,
    awakeTimeMinutesMax: 90,
    targetNapCount: 4,
    targetNapHours: 5,
    targetAwakeHours: 5,
    targetNightHours: 11,
    targetTotalSleepHours: 16,
  },

  KEYS: [
    "Score",
    "Nap Count",
    "Nap Hours",
    "Awake Hours",
    "Night Hours",
    "Sleep Hours",
    "06:00", "06:30", "07:00", "07:30", "08:00", "08:30", "09:00", "09:30", "10:00", "10:30",
    "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30", "15:00", "15:30",
    "16:00", "16:30", "17:00", "17:30", "18:00", "18:30", "19:00", "19:30", "20:00",
  ],

  parseTime(text) {
    if (!text) return null;
    const match = /^([01]?\d|2[0-3]):([0-5]\d)$/.exec(text.trim());
    if (!match) return null;
    return Number(match[1]) * 60 + Number(match[2]);
  },

  formatTime(minutes) {
    const h = String(Math.floor(minutes / 60)).padStart(2, "0");
    const m = String(minutes % 60).padStart(2, "0");
    return `${h}:${m}`;
  },

  scheduleHours(schedule, activity) {
    return schedule
      .filter((item) => item.activity === activity)
      .reduce((sum, item) => sum + (item.end - item.start), 0) / 60;
  },

  getNightHours(schedule) {
    return this.scheduleHours(schedule, "NightTime") + 24 - (this.CONFIG.endOfSchedule - this.CONFIG.startOfSchedule) / 60;
  },

  getActivitiesIn30MinuteSpans(schedule) {
    const spans = [];
    let time = schedule[0].start;
    const end = schedule[schedule.length - 1].end;
    let index = 0;

    while (time < end) {
      while (index < schedule.length && time >= schedule[index].end) {
        index++;
      }
      if (index < schedule.length) {
        spans.push(schedule[index].activity);
      }
      time += 30;
    }

    return spans;
  },

  calculateScore(actual, target) {
    return Math.max(0, 1000 - 100 * Math.abs(actual - target));
  },

  buildMetadata(schedule) {
    const numberOfNaps = schedule.filter((item) => item.activity === "Nap").length;
    const napHours = this.scheduleHours(schedule, "Nap");
    const awakeHours = this.scheduleHours(schedule, "Awake");
    const nightHours = this.getNightHours(schedule);
    const totalSleepHours = nightHours + napHours;

    const score = Math.trunc(
      this.calculateScore(numberOfNaps, this.CONFIG.targetNapCount)
      + this.calculateScore(napHours, this.CONFIG.targetNapHours)
      + this.calculateScore(awakeHours, this.CONFIG.targetAwakeHours)
      + this.calculateScore(nightHours, this.CONFIG.targetNightHours)
      + this.calculateScore(totalSleepHours, this.CONFIG.targetTotalSleepHours)
    );

    return {
      score,
      numberOfNaps,
      napHours,
      awakeHours,
      nightHours,
      totalSleepHours,
      activitiesIn30MinuteSpans: this.getActivitiesIn30MinuteSpans(schedule),
    };
  },

  toTable(metadatas) {
    const rows = Array.from({ length: this.KEYS.length + 1 }, () => []);
    rows[0].push("");

    metadatas.forEach((_, i) => rows[0].push(`Schedule ${i + 1}`));

    this.KEYS.forEach((key, rowIndex) => {
      rows[rowIndex + 1].push(key);
      metadatas.forEach((meta) => {
        const values = [
          String(meta.score),
          String(meta.numberOfNaps),
          String(meta.napHours),
          String(meta.awakeHours),
          String(meta.nightHours),
          String(meta.totalSleepHours),
          ...meta.activitiesIn30MinuteSpans,
        ];
        rows[rowIndex + 1].push(values[rowIndex] || "");
      });
    });

    return rows;
  },

  isValid(schedule, typedFilters) {
    if (!typedFilters.length) return true;

    const startsMap = new Map();
    const endsMap = new Map();

    schedule.forEach((item) => {
      if (!startsMap.has(item.activity)) startsMap.set(item.activity, new Set());
      if (!endsMap.has(item.activity)) endsMap.set(item.activity, new Set());
      startsMap.get(item.activity).add(item.start);
      endsMap.get(item.activity).add(item.end);
    });

    return typedFilters.every((filter) => {
      const last = schedule[schedule.length - 1];

      if (filter.action === "Starts") {
        return filter.time > last.start || (startsMap.has(filter.activity) && startsMap.get(filter.activity).has(filter.time));
      }

      if (filter.action === "Ends") {
        return filter.time > last.end || (endsMap.has(filter.activity) && endsMap.get(filter.activity).has(filter.time));
      }

      return false;
    });
  },

  addNap(results, schedule, typedFilters) {
    const lastEnd = schedule[schedule.length - 1].end;
    if (!this.isValid(schedule, typedFilters) || lastEnd > this.CONFIG.bedTimeMax) {
      return;
    }

    if (lastEnd >= this.CONFIG.bedTimeMin) {
      results.push([...schedule, { start: lastEnd, end: this.CONFIG.endOfSchedule, activity: "NightTime" }]);
      return;
    }

    for (let napMinutes = this.CONFIG.napTimeMinutesMax; napMinutes >= this.CONFIG.napTimeMinutesMin; napMinutes -= this.CONFIG.timeIncrementMinutes) {
      const start = schedule[schedule.length - 1].end;
      this.addAwake(results, [...schedule, { start, end: start + napMinutes, activity: "Nap" }], typedFilters);
    }
  },

  addAwake(results, schedule, typedFilters) {
    const lastEnd = schedule[schedule.length - 1].end;
    if (!this.isValid(schedule, typedFilters) || lastEnd > this.CONFIG.bedTimeMax) {
      return;
    }

    for (let awakeMinutes = this.CONFIG.awakeTimeMinutesMax; awakeMinutes >= this.CONFIG.awakeTimeMinutesMin; awakeMinutes -= this.CONFIG.timeIncrementMinutes) {
      const start = schedule[schedule.length - 1].end;
      this.addNap(results, [...schedule, { start, end: start + awakeMinutes, activity: "Awake" }], typedFilters);
    }
  },

  generate(rawFilters) {
    const typedFilters = rawFilters
      .filter((f) => ["NightTime", "Awake", "Nap"].includes(f.activity) && ["Starts", "Ends"].includes(f.action) && this.parseTime(f.time) !== null)
      .map((f) => ({ activity: f.activity, action: f.action, time: this.parseTime(f.time) }));

    const schedules = [];
    for (let wakeUp = this.CONFIG.wakeUpTimeMax; wakeUp >= this.CONFIG.wakeUpTimeMin; wakeUp -= this.CONFIG.timeIncrementMinutes) {
      this.addAwake(schedules, [{ start: this.CONFIG.startOfSchedule, end: wakeUp, activity: "NightTime" }], typedFilters);
    }

    return schedules;
  },
};

// ========================================
// UI LAYER - Presentation & Events
// ========================================

const filtersContainer = document.getElementById("filters");
const scheduleTable = document.getElementById("scheduleTable");
const addFilterButton = document.getElementById("addFilter");
const applyFiltersButton = document.getElementById("applyFilters");
const helpButton = document.getElementById("helpButton");
const helpModal = document.getElementById("helpModal");
const closeHelpButton = document.getElementById("closeHelpButton");
const loadingIndicator = document.getElementById("loadingIndicator");
const scheduleCountElement = document.getElementById("scheduleCount");
const countNumberElement = document.getElementById("countNumber");
const filtersPanel = document.getElementById("filtersPanel");
const collapseToggle = document.getElementById("collapseToggle");
const filterSummaryText = document.getElementById("filterSummaryText");

let filters = [];

function getActiveFilterLabels() {
  const active = filters.filter(
    (filter) => filter.activity && filter.action && ScheduleEngine.parseTime(filter.time) !== null
  );

  if (!active.length) {
    return "No constraints applied. Click to open filters.";
  }

  return `Filters: ${active
    .map((filter) => `${filter.activity} ${filter.action} ${ScheduleEngine.formatTime(ScheduleEngine.parseTime(filter.time))}`)
    .join(" · ")}`;
}

function updateFilterSummary() {
  filterSummaryText.textContent = getActiveFilterLabels();
  const expanded = !filtersPanel.classList.contains("collapsed");
  collapseToggle.textContent = expanded ? "Hide filters" : "Show filters";
  collapseToggle.setAttribute("aria-expanded", String(expanded));
}

helpButton.addEventListener("click", () => helpModal.classList.add("active"));
closeHelpButton.addEventListener("click", () => helpModal.classList.remove("active"));
helpModal.addEventListener("click", (e) => {
  if (e.target === helpModal) {
    helpModal.classList.remove("active");
  }
});

function parseFiltersPath(path) {
  if (!path) return [];
  const segments = path.split("/").filter(Boolean);
  if (segments.length % 3 !== 0) return [];
  const parsed = [];
  for (let i = 0; i < segments.length; i += 3) {
    parsed.push({ activity: segments[i], action: segments[i + 1], time: segments[i + 2] });
  }
  return parsed;
}

function buildFiltersPath(currentFilters) {
  return currentFilters
    .filter((f) => f.activity && f.action && ScheduleEngine.parseTime(f.time) !== null)
    .map((f) => `${f.activity}/${f.action}/${ScheduleEngine.formatTime(ScheduleEngine.parseTime(f.time))}`)
    .join("/");
}

function getFiltersPathFromUrl() {
  const url = new URL(window.location.href);
  return (url.searchParams.get("filters") || "").trim();
}

function reflectUrl(filtersPath, replace = false) {
  const url = new URL(window.location.href);
  if (filtersPath) {
    url.searchParams.set("filters", filtersPath);
  } else {
    url.searchParams.delete("filters");
  }
  if (replace) {
    history.replaceState(null, "", url);
  } else {
    history.pushState(null, "", url);
  }
}

function renderFilters() {
  filtersContainer.innerHTML = "";
  const fragment = document.createDocumentFragment();
  const activities = ["NightTime", "Awake", "Nap"];
  const actions = ["Starts", "Ends"];
  filters.forEach((filter, index) => {
    const row = document.createElement("div");
    row.className = "filter-row";
    row.innerHTML = `
      <select data-index="${index}" data-type="activity">
        <option value="">Activity</option>
        ${activities.map((activity) => `<option value="${activity}" ${filter.activity === activity ? "selected" : ""}>${activity}</option>`).join("")}
      </select>
      <select data-index="${index}" data-type="action">
        <option value="">Action</option>
        ${actions.map((action) => `<option value="${action}" ${filter.action === action ? "selected" : ""}>${action}</option>`).join("")}
      </select>
      <input data-index="${index}" data-type="time" type="time" step="1800" value="${filter.time}" />
      <button type="button" data-index="${index}" data-type="remove">×</button>
    `;
    fragment.appendChild(row);
  });
  filtersContainer.appendChild(fragment);
}

function renderTable(rows) {
  scheduleTable.innerHTML = "";
  const fragment = document.createDocumentFragment();
  const thead = document.createElement("thead");
  const headerRow = document.createElement("tr");
  rows[0].forEach((cell) => {
    const th = document.createElement("th");
    th.textContent = cell;
    headerRow.appendChild(th);
  });
  thead.appendChild(headerRow);
  fragment.appendChild(thead);

  const tbody = document.createElement("tbody");
  rows.slice(1).forEach((row) => {
    const tr = document.createElement("tr");
    row.forEach((cell) => {
      const td = document.createElement("td");
      td.textContent = cell;
      if (["Nap", "Awake", "NightTime"].includes(cell)) {
        td.classList.add(cell);
      }
      tr.appendChild(td);
    });
    tbody.appendChild(tr);
  });
  fragment.appendChild(tbody);
  scheduleTable.appendChild(fragment);
}

function normalizeFilters(rawFilters) {
  const activities = ["NightTime", "Awake", "Nap"];
  const actions = ["Starts", "Ends"];
  return rawFilters.map((filter) => ({
    activity: activities.includes(filter.activity) ? filter.activity : "",
    action: actions.includes(filter.action) ? filter.action : "",
    time: ScheduleEngine.parseTime(filter.time) === null ? "" : ScheduleEngine.formatTime(ScheduleEngine.parseTime(filter.time)),
  }));
}

filtersContainer.addEventListener("change", (event) => {
  const index = Number(event.target.dataset.index);
  const type = event.target.dataset.type;
  if (Number.isNaN(index) || !type) return;
  filters[index][type] = event.target.value;
});

filtersContainer.addEventListener("click", (event) => {
  if (event.target.dataset.type !== "remove") return;
  const index = Number(event.target.dataset.index);
  if (Number.isNaN(index)) return;
  filters.splice(index, 1);
  renderFilters();
});

addFilterButton.addEventListener("click", () => {
  filters.push({ activity: "", action: "", time: "" });
  renderFilters();
});

applyFiltersButton.addEventListener("click", () => {
  refresh(false);
});

collapseToggle.addEventListener("click", () => {
  filtersPanel.classList.toggle("collapsed");
  collapseToggle.classList.toggle("collapsed");
  updateFilterSummary();
});

function refresh(replaceHistory = false) {
  loadingIndicator.style.display = "flex";
  filters = normalizeFilters(filters);
  const filtersPath = buildFiltersPath(filters);
  reflectUrl(filtersPath, replaceHistory);

  const schedules = ScheduleEngine.generate(filters)
    .map((schedule) => ScheduleEngine.buildMetadata(schedule))
    .sort((a, b) => b.score - a.score);

  renderTable(ScheduleEngine.toTable(schedules));

  if (schedules.length > 0) {
    scheduleCountElement.style.display = "block";
    countNumberElement.textContent = schedules.length.toLocaleString();
  } else {
    scheduleCountElement.style.display = "none";
  }
  loadingIndicator.style.display = "none";
  updateFilterSummary();
}

(function init() {
  filters = normalizeFilters(parseFiltersPath(getFiltersPathFromUrl()));
  renderFilters();
  refresh(true);
})();
