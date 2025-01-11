# simpli-search

## Technical stack

### Infrastructure

- **[`.NET Aspire`](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)** - .NET Aspire is an opinionated, cloud ready stack for building observable, production ready, distributed applications.
- **[`Kubernetes`](https://kubernetes.io)** - The app is designed to run on Kubernetes (both locally as well as on the cloud)

### Front-end

- **[`Refine`](https://refine.dev)** - Refine is a React meta-framework for CRUD-heavy web applications. It addresses a wide range of enterprise use cases including internal tools, admin panels, dashboards and B2B apps.
- **[`Next.js`](https://nextjs.org)** - A modern server side rendering for React application

### Back-end

- **[`.NET Core 9`](https://dotnet.microsoft.com/download)** - .NET Framework and .NET Core, including ASP.NET and ASP.NET Core
- **[`EF Core 9`](https://github.com/dotnet/efcore)** - Modern object-database mapper for .NET. It supports LINQ queries, change tracking, updates, and schema migrations
- **[`MediatR`](https://github.com/jbogard/MediatR)** - Simple, unambitious mediator implementation in .NET

### Testing

- **[`TestContainer`](https://testcontainers.com/guides/getting-started-with-testcontainers-for-dotnet)** - Testcontainers is a testing library that provides easy and lightweight APIs for bootstrapping integration tests with real services wrapped in Docker containers


### CI & CD

- **[`GitHub Actions`](https://github.com/features/actions)**
- **[`Flux CD`](https://fluxcd.io/)** - Flux CD automates Kubernetes deployment from Git, ensuring continuous delivery seamlessly.
- **[`SonarCloud`](https://sonarcloud.io/)**


### Charts

```mermaid
flowchart TD
    %% Client Layer
    subgraph Client Layer
        A[Client - CEO or CTO]:::client -->|Step 1: Enter Keywords and URL| B[Load Balancer]:::loadbalancer
    end

    %% Web Server Layer
    subgraph Web Server Layer
        B -->|Step 2: Distribute Requests| C[API Gateway]:::gateway
        C -->|Step 3: Verify Authentication| D[Authentication Service]:::auth
        C -->|Step 4: Forward to Search Service| E[Search Service]:::searchservice
    end

    %% Search Service Layer
    subgraph Search Service Layer
        E -->|Step 5: Check Cache| F[[Redis Cache]]:::cache

        F -->|Cache Miss| H[Search Engine Factory]:::factory

        subgraph Scrapers
            H --> H1[Google Scraper]:::scraper
            H --> H2[Bing Scraper]:::scraper
            H --> H3[Other Scrapers]:::scraper
        end

        H1 -->|Step 6: Parse Results| I[Result Processor]:::processor
        H2 --> I
        H3 --> I

        I -->|Step 7: Save to Database| N[(Database)]:::database
        I -->|Step 8: Update Cache| F


        I -->|Step 10: Publish to Message Broker| MB[Message Broker]:::broker
    end

    %% Optional Extensions Layer
    subgraph Optional Extensions Layer
        MB -->|Notify| L[Analytics Service]:::analytics
        MB -->|Notify| M[Notification Service]:::notifications
    end

    F -->|Cache Hit| G[Return Cached Results]:::output
    I -->|Step 9: Return Results| K[Client Response]:::output

    %% Styling
    classDef client fill:#1976D2,stroke:#004BA0,stroke-width:2px,color:#FFF; %% Material Blue 700
    classDef loadbalancer fill:#0288D1,stroke:#01579B,stroke-width:2px,color:#FFF; %% Material Light Blue 700
    classDef gateway fill:#00796B,stroke:#004D40,stroke-width:2px,color:#FFF; %% Material Teal 700
    classDef auth fill:#388E3C,stroke:#1B5E20,stroke-width:2px,color:#FFF; %% Material Green 700
    classDef searchservice fill:#F57C00,stroke:#E65100,stroke-width:2px,color:#FFF; %% Material Orange 700
    classDef cache fill:#D32F2F,stroke:#B71C1C,stroke-width:2px,color:#FFF,shape:hexagon; %% Material Red 700
    classDef factory fill:#512DA8,stroke:#311B92,stroke-width:2px,color:#FFF; %% Material Deep Purple 700
    classDef scraper fill:#7B1FA2,stroke:#4A148C,stroke-width:2px,color:#FFF; %% Material Purple 700
    classDef processor fill:#5D4037,stroke:#3E2723,stroke-width:2px,color:#FFF; %% Material Brown 700
    classDef broker fill:#FFC107,stroke:#FFA000,stroke-width:2px,color:#000,shape:cylinder; %% Material Amber 700
    classDef output fill:#388E3C,stroke:#1B5E20,stroke-width:2px,color:#FFF; %% Material Green 700
    classDef analytics fill:#1976D2,stroke:#004BA0,stroke-width:2px,color:#FFF; %% Material Blue 700
    classDef notifications fill:#E64A19,stroke:#BF360C,stroke-width:2px,color:#FFF; %% Material Deep Orange 700
    classDef database fill:#3E2723,stroke:#1B1B1B,stroke-width:2px,color:#FFF,shape:subroutine; %% Material Dark Brown
```
