# Napper

Napper is a lightweight browser-based sleep schedule optimizer. It generates all valid schedules based on fixed day boundaries, nap/awake cycles, and a choice of user constraints supplied through the UI or URL filters.

## Features

- Generates every valid schedule that fits within the app's sleep model.
- Displays results in a readable table with color-coded activities.
- Supports filters by activity/time to narrow the result set.
- Keeps filters in the URL so results are shareable and reproducible.
- Uses 30-minute time increments for all schedule events.
- Includes a compact header summary and expandable constraints panel.

## How Napper Works

Napper builds schedules using the following rules:

- The day starts between `06:30` and `08:30`.
- Sleep must begin as `NightTime` before wake-up.
- After waking, the schedule alternates between `Awake` and `Nap` blocks.
- `Nap` durations can be `1.0`, `1.5`, or `2.0` hours.
- `Awake` durations are `1.0` or `1.5` hours.
- The final block may end into the defined evening `NightTime` period.
- The schedule ends by `20:30`.

The app scores schedules and presents them in a table where each column shows one candidate schedule and each row represents either metadata or a 30-minute timeslot.

## Filter Syntax and URL Example

You can apply constraints through the URL using the `filters` query parameter. The filter format is:

`Activity/Action/Time` repeated for each constraint.

Examples:

- `NightTime/Ends/07:00` — require wake-up by 07:00.
- `Nap/Starts/08:30` — require a nap to begin at 08:30.
- `Nap/Ends/10:30` — require a nap to end at 10:30.

### Demo URL with Applied Filters

Open the app with this example filter set:

`index.html?filters=NightTime/Ends/07:00/Nap/Starts/08:30/Nap/Ends/10:30/Nap/Ends/13:30/Nap/Ends/18:30`

This URL automatically loads constraints into the UI and displays matching schedules immediately.

## Usage

1. Open `index.html` in your browser.
2. Click the question mark icon to view help and details.
3. Use the compact header summary to see active filters.
4. Expand the constraints panel to add or edit filters.
5. Click `Apply` to refresh schedule results.

## Development Notes

- `app.js` contains the schedule generation and UI logic.
- `styles.css` contains the layout, table styling, and responsive behavior.
- `index.html` is the application shell and includes the help modal.

## Notes

- The schedule table uses color coding for `Nap`, `Awake`, and `NightTime` cells.
- Results are sorted by score so the most balanced schedules appear first.
- The URL filter format is compatible with the existing query string approach from earlier versions.