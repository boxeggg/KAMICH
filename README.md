# KAMICH

**Cross-platform work-hours and revenue tracker for a rail-road excavator rental business, built on live telemetry instead of spreadsheets.**

[![Release](https://img.shields.io/github/v/release/boxeggg/KAMICH)](https://github.com/boxeggg/KAMICH/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> ⚠️ This app is not meant to be installed by random visitors. It requires a valid **Linqo** API key belonging to the excavator's telematics account to do anything useful. It's shared here as a portfolio project. Screenshots coming soon.

## The problem

A local company rents out a rail-road excavator (a two-way machine that runs on both rail and road). Every operating hour is billable, but without a system, tracking how many hours the machine actually worked each day, week, month and year, and what that translates to in revenue, meant manual logs, guesswork, and delayed invoicing.

**KAMICH closes that gap.** It ingests operating telemetry automatically through a **Linqo** fleet-telematics integration, aggregates it, and turns raw working hours into daily/monthly/yearly statistics and revenue simulations, on your phone or on a desktop dashboard.

## Tech stack

| Layer | Technology |
|---|---|
| Mobile & desktop client | .NET MAUI (C#), single codebase for a native Android app and a desktop layout |
| Backend API | Java, Spring Boot |
| Database | PostgreSQL |
| External integration | Linqo fleet telematics API |
| Hosting | Railway |
| CI/CD | GitHub Actions, tag-triggered build, signing, and self-distributing release pipeline |

## Architecture

\`\`\`mermaid
flowchart LR
    EXC[Rail-road excavator] -->|telemetry| LINQO[Linqo Telematics API]
    LINQO -->|polled| API[Spring Boot REST API]
    API <--> DB[(PostgreSQL)]
    API -->|REST| MAUI[.NET MAUI App]
    MAUI --> AND[Android]
    MAUI --> DESK[Desktop]
\`\`\`

An earlier version had the MAUI client talking to Linqo directly. It was refactored so the Spring Boot backend owns the Linqo integration, and the client only talks to a clean internal REST API. Telemetry parsing and business logic live in one place instead of being duplicated across platforms.

## Deployment and CI/CD

The whole release pipeline is automated end to end:

1. Pushing a **tag** to `master` triggers a **GitHub Actions** workflow.
2. The workflow builds and **signs the Android APK** with a private key, then publishes it as a GitHub Release.
3. The **Spring Boot API** periodically checks GitHub for a new release, downloads the APK, and caches it.
4. From there, the API distributes the update over the air to devices already running the app. No manual APK sideloading needed after the first install.
5. The API itself runs on **Railway**.

## Core features

- 📡 **Automatic data ingestion**: telemetry is reported straight to the backend through the Linqo integration, nothing is logged by hand.
- 📊 **Daily statistics on the home screen**: today's worked hours at a glance.
- 📈 **Trend tracking**: aggregated worked hours across day, month, and year.
- 💰 **Revenue simulation**: projected earnings from tracked hours using configurable rates.
- 🔄 **Live working indicator**: visual feedback while data syncs in the background.
- 🚀 **Self-updating client**: new APKs roll out to installed devices automatically via the backend, no app store needed.

## License

MIT. Free to use, modify and deploy.

---

*Solo-developed end to end: Linqo telemetry integration, Spring Boot API on Railway, PostgreSQL schema, tag-triggered CI/CD with self-distributing OTA updates, and the .NET MAUI client for both Android and desktop.*
