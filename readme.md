- The output returns all different possible schedules
- The output is in CSV format. It contains start time, end time, activity
- The only possible activities are “Nap” or “Awake”
- A schedule must alternate between these 2. Having consecutive naps, or awake times is not allowed.
- The schedule goes in 30 minutes increments
- The day starts between 6.30 and 8
- Nap can last 1, 1.5 or 2 hours
- Awake can last 1 or 1.5 hours
- The schedule must finish with a Nap
- The last Nap must finish at 18.30
- On a given schedule, there might be different lengths of nap and awake time

https://localhost:44422/NightTime/Ends/07:00/Nap/Starts/8:30/Nap/Ends/10:30/Nap/Ends/13:30/Nap/Ends/18:30