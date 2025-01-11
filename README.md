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
        E -->|Step 5: Check for Cached Results| F[Redis Cache]:::cache
        F -->|Cache Hit| G[Return Cached Results to Client]:::output
        F -->|Cache Miss| H[Search Engine Handler]:::handler

        subgraph Search Engine Scrapers
            H --> H1[Google Scraper]:::scraper
            H --> H2[Bing Scraper]:::scraper
            H --> H3[Other Search Engines]:::scraper
        end

        H1 -->|Step 6: Parse Results| I[Result Parser]:::parser
        H2 -->|Step 6: Parse Results| I
        H3 -->|Step 6: Parse Results| I
    end

    %% Result Processing Layer
    subgraph Result Processing Layer
        I -->|Step 7: Update Cache| F
        I -->|Step 8: Format Results| J[Result Formatter]:::formatter
        J -->|Step 9: Return Results to Client| K[Client Response]:::output
    end

    %% Optional Extensions Layer
    subgraph Optional Extensions Layer
        E -->|Step 10: Generate Analytics| L[Analytics Service]:::analytics
        E -->|Step 11: Notify CEO of Updates| M[Notification Service]:::notifications
    end

    %% Database Layer
    subgraph Database Layer
        E -->|Step 12: Save Search Logs| N[Search History Database]:::database
        F -->|Step 13: Backup Cache Data| N
    end

    %% Styling
    classDef client fill:#FFD700,stroke:#333,stroke-width:2px,color:#000;
    classDef loadbalancer fill:#87CEEB,stroke:#333,stroke-width:2px,color:#000;
    classDef gateway fill:#FFA07A,stroke:#333,stroke-width:2px,color:#000;
    classDef auth fill:#B0E0E6,stroke:#333,stroke-width:2px,color:#000;
    classDef searchservice fill:#90EE90,stroke:#333,stroke-width:2px,color:#000;
    classDef cache fill:#FF6347,stroke:#333,stroke-width:2px,color:#FFF;
    classDef handler fill:#4682B4,stroke:#333,stroke-width:2px,color:#FFF;
    classDef scraper fill:#6A5ACD,stroke:#333,stroke-width:2px,color:#FFF;
    classDef parser fill:#DA70D6,stroke:#333,stroke-width:2px,color:#000;
    classDef formatter fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000;
    classDef output fill:#32CD32,stroke:#333,stroke-width:2px,color:#FFF;
    classDef analytics fill:#8A2BE2,stroke:#333,stroke-width:2px,color:#FFF;
    classDef notifications fill:#FF4500,stroke:#333,stroke-width:2px,color:#FFF;
    classDef database fill:#8B4513,stroke:#FFF,stroke-width:2px,color:#FFF;
```
