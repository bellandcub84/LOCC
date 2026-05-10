# LOCC - Live Outbreak Command Centre (MVP)

This repository contains the MVP scaffold for LOCC — an AIIMS-aligned outbreak and recovery command system for Australian residential aged care facilities.

This initial commit contains:
- Domain models for facilities, residents, staff, locations, outbreaks, cases, tests, tasks, resources, communications, recovery and evidence models.
- A simple EF Core SQLite-backed DbContext and seed data for a demo facility (Rosewood Aged Care).
- A simple rule engine that evaluates outbreak data and creates alerts, recommendations and AIIMS-aligned tasks.
- A minimal API Program.cs that seeds data and runs rule evaluations on startup (prints results to console).

See src/ for projects.
