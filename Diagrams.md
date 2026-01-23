

# VRVis XR Twin - Topic Tree / Feature Overview

Diese Darstellung zeigt die wichtigsten Themen des Projekts aus der Vogelperspektive.

---

## Topic Tree

```mermaid
%%{init: {'theme': 'default', 'themeVariables': { 'fontSize': '15px'}}}%%
mindmap
  root((VRVis XR Twin))
    Core Systems
      ServiceLocator
        Service Registry
        DontDestroyOnLoad
      AppController
        Scene Loading
        App Lifecycle
      ConfigurationService
        appconfig.json
        User Settings
    UI System
      UIManager
        Window Lifecycle
        UI Regions
      MVP Pattern
        Presenter
        View
        Model
      UIToolkit
        UXML Layouts
        USS Styles
    Geospatial 3D
      LayerManager
        Projects
        Variants
        Base Layers
      Cesium
        3D Tiles
        Tile Streaming
        LOD Management
      Caching
        LayerDownloader
        ProjectDownloader
    XR / VR
      PlayerController
        Input Handling
        Mode Switching
      OpenXR
        HMD Detection
        Controller Profiles
      Locomotion
        Teleportation
        Continuous Move
    Networking
      UGS
        Authentication
        Cloud Services
      Vivox
        Voice Chat
        Text Commands
      Collaboration
        Meetings
        Command Sync
    Authentication
      OAuth2
        PKCE Flow
        Token Refresh
      Permissions
        Owner
        Moderator
        User
```










---









## Hierarchical Flowchart

```mermaid
%%{init: {'theme': 'dark', 'themeVariables': { 'primaryColor': '#4A90D9', 'primaryTextColor': '#fff'}}}%%
flowchart TB
    VRVis["<b>VRVis XR Twin</b><br/>Collaborative XR Digital Twin"]

    VRVis --> CORE
    VRVis --> UI
    VRVis --> GEO
    VRVis --> XR
    VRVis --> NET
    VRVis --> AUTH

    subgraph CORE["CORE SYSTEMS"]
        C1["<b>ServiceLocator</b>"]
        C2["<b>AppController</b>"]
        C3["<b>Configuration</b>"]
        C1a["Service Registry"]
        C2a["Scene Loading"]
        C3a["appconfig.json"]
    end

    subgraph UI["UI SYSTEM"]
        U1["<b>UIManager</b>"]
        U2["<b>MVP Pattern</b>"]
        U3["<b>UIToolkit</b>"]
        U1a["Window Lifecycle"]
        U2a["Presenter/View/Model"]
        U3a["UXML/USS"]
    end

    subgraph GEO["GEOSPATIAL"]
        G1["<b>LayerManager</b>"]
        G2["<b>Cesium</b>"]
        G3["<b>Caching</b>"]
        G1a["Projects & Variants"]
        G2a["3D Tile Streaming"]
        G3a["Offline Support"]
    end

    subgraph XR["XR / VR"]
        X1["<b>PlayerController</b>"]
        X2["<b>OpenXR</b>"]
        X3["<b>Locomotion</b>"]
        X1a["Input Handling"]
        X2a["HMD Detection"]
        X3a["Teleport & Move"]
    end

    subgraph NET["NETWORKING"]
        N1["<b>UGS</b>"]
        N2["<b>Vivox</b>"]
        N3["<b>Collaboration</b>"]
        N1a["Cloud Services"]
        N2a["Voice & Commands"]
        N3a["Meeting Sync"]
    end

    subgraph AUTH["AUTH"]
        A1["<b>OAuth2</b>"]
        A2["<b>Permissions</b>"]
        A1a["PKCE + Tokens"]
        A2a["Role-based Access"]
    end

    C1 --> C1a
    C2 --> C2a
    C3 --> C3a
    U1 --> U1a
    U2 --> U2a
    U3 --> U3a
    G1 --> G1a
    G2 --> G2a
    G3 --> G3a
    X1 --> X1a
    X2 --> X2a
    X3 --> X3a
    N1 --> N1a
    N2 --> N2a
    N3 --> N3a
    A1 --> A1a
    A2 --> A2a

    classDef main fill:#2C3E50,stroke:#1A252F,color:#fff,font-weight:bold
    classDef core fill:#4A90D9,stroke:#2D5F8B,color:#fff
    classDef ui fill:#28A745,stroke:#1E7E34,color:#fff
    classDef geo fill:#FFC107,stroke:#D39E00,color:#000
    classDef xr fill:#6F42C1,stroke:#5A32A3,color:#fff
    classDef net fill:#17A2B8,stroke:#117A8B,color:#fff
    classDef auth fill:#DC3545,stroke:#BD2130,color:#fff
    classDef sub fill:#E9ECEF,stroke:#ADB5BD,color:#333

    class VRVis main
    class C1,C2,C3 core
    class U1,U2,U3 ui
    class G1,G2,G3 geo
    class X1,X2,X3 xr
    class N1,N2,N3 net
    class A1,A2 auth
    class C1a,C2a,C3a,U1a,U2a,U3a,G1a,G2a,G3a,X1a,X2a,X3a,N1a,N2a,N3a,A1a,A2a sub
```






---












# Feature Map / Node Graph

Diese Feature Map zeigt die Haupt-Subsysteme und ihre Beziehungen. Nutze sie als Orientierung während Q&A und zur Navigation durch die Codebase.

## Mermaid Diagram

```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'primaryColor': '#4A90D9', 'primaryTextColor': '#fff', 'primaryBorderColor': '#2D5F8B', 'lineColor': '#666', 'secondaryColor': '#6C757D', 'tertiaryColor': '#E9ECEF'}}}%%

flowchart TB
    subgraph CORE["🎯 CORE SYSTEMS"]
        SL["<b>ServiceLocator</b><br/>Service Registry"]
        AC["<b>AppController</b><br/>App Orchestration"]
        CFG["<b>ConfigurationService</b><br/>appconfig.json"]
    end

    subgraph UI["🖥️ UI SYSTEM"]
        UIM["<b>UIManager</b><br/>Window Lifecycle"]
        MVP["<b>MVP Pattern</b><br/>Presenter/View/Model"]
        UITK["UIToolkit<br/>UXML/USS"]
        REG["UI Regions<br/>Header/Sidebar/Content"]
    end

    subgraph GEO["🌍 GEOSPATIAL"]
        LM["<b>LayerManager</b><br/>Cesium Orchestration"]
        LD["LayerDownloader<br/>Tile Caching"]
        PD["ProjectDownloader<br/>Asset Caching"]
        CES["Cesium 3DTileset<br/>Tile Streaming"]
    end

    subgraph XR["🥽 XR SYSTEM"]
        PC["<b>PlayerController</b><br/>Input Handling"]
        HMD["HmdPresenceMonitor<br/>Device Detection"]
        TELE["Teleportation<br/>Locomotion"]
        OXR["OpenXR<br/>Runtime Abstraction"]
    end

    subgraph NET["🔗 NETWORKING"]
        UGS["<b>UGSService</b><br/>Unity Gaming Services"]
        VIV["VivoxHandler<br/>Voice/Commands"]
        CMD["CommandBusService<br/>Rate Limiting"]
        COL["CollaborationService<br/>Meetings"]
    end

    subgraph AUTH["🔐 AUTH"]
        OA["OAuth2Service<br/>PKCE Flow"]
        PERM["PermissionService<br/>Roles"]
        TOK["TokenStorage<br/>Windows Credential"]
    end

    %% Core connections
    SL --> CFG
    SL --> AC
    AC --> UIM
    AC --> LM

    %% UI connections
    UIM --> MVP
    MVP --> UITK
    UIM --> REG

    %% Geo connections
    LM --> LD
    LM --> PD
    LM --> CES

    %% XR connections
    PC --> HMD
    PC --> TELE
    HMD --> OXR

    %% Network connections
    UGS --> VIV
    VIV --> CMD
    CMD --> COL
    COL --> LM

    %% Auth connections
    OA --> PERM
    OA --> TOK
    PERM --> UIM

    %% Cross-system connections
    CFG -.-> LM
    CFG -.-> OA
    CFG -.-> UGS
    HMD -.-> UIM
    COL -.-> PC

    classDef core fill:#4A90D9,stroke:#2D5F8B,color:#fff
    classDef ui fill:#28A745,stroke:#1E7E34,color:#fff
    classDef geo fill:#FFC107,stroke:#D39E00,color:#000
    classDef xr fill:#6F42C1,stroke:#5A32A3,color:#fff
    classDef net fill:#17A2B8,stroke:#117A8B,color:#fff
    classDef auth fill:#DC3545,stroke:#BD2130,color:#fff
    classDef small fill:#E9ECEF,stroke:#ADB5BD,color:#000

    class SL,AC,CFG core
    class UIM,MVP ui
    class LM geo
    class PC,HMD xr
    class UGS,COL net
    class OA,PERM auth
    class UITK,REG,LD,PD,CES,TELE,OXR,VIV,CMD,TOK small
```

---

## Legende

| Farbe | Subsystem | Hauptdateien |
|-------|-----------|--------------|
| 🔵 Blau | **Core Systems** | ServiceLocator.cs, AppController.cs, ConfigurationService.cs |
| 🟢 Grün | **UI System** | UIManager.cs, PresenterBase.cs, ViewBase.cs |
| 🟡 Gelb | **Geospatial** | LayerManager.cs, LayerDownloader.cs, Cesium Prefabs |
| 🟣 Lila | **XR System** | PlayerController.cs, HmdPresenceMonitorService.cs |
| 🔵 Cyan | **Networking** | UGSService.cs, VivoxHandler.cs, CollaborationService.cs |
| 🔴 Rot | **Auth** | OAuth2AuthenticationService.cs, PermissionService.cs |

---











## Big Topics

Diese Themen erfordern tieferes Verständnis:

1. **ServiceLocator** - Zentrale Service-Registry, DontDestroyOnLoad
2. **AppController** - Scene Loading, UI Setup, App Lifecycle
3. **ConfigurationService** - appconfig.json, User Settings
4. **UIManager** - Window Lifecycle, Regions, MVP Orchestration
5. **MVP Pattern** - Presenter/View/Model Architektur
6. **LayerManager** - Cesium Layers, Projects, Variants
7. **PlayerController** - Input Handling, XR/Desktop Switch
8. **HmdPresenceMonitor** - VR Device Detection
9. **UGSService** - Unity Gaming Services Facade
10. **CollaborationService** - Meetings, Voice, Commands

---

## Small Topics 

Diese Themen sind Detail-Aspekte:

- **UIToolkit** - UXML, USS, UIDocument
- **UI Regions** - Header, Sidebar, Content1, Overlay, Toast
- **LayerDownloader** - 3D Tiles Caching
- **ProjectDownloader** - glTF/GLB Asset Caching
- **Cesium 3DTileset** - Native Tile Streaming
- **Teleportation** - XR + Keyboard Teleport
- **OpenXR** - Runtime Abstraction Layer
- **VivoxHandler** - Voice Channels, Commands
- **CommandBusService** - Rate Limiting, Event Dispatch
- **TokenStorage** - Windows Credential Manager
- **PermissionService** - Owner/Moderator/User Roles










---














# Architecture Diagrams

## 1. Bootstrap / Runtime Flow

```mermaid
flowchart TD
    A[Startup.unity lädt] --> B[ServiceLocator.Awake]
    B --> C[DontDestroyOnLoad]
    C --> D[RegisterServicesAsync]

    D --> D1[ConfigurationService<br/>lädt appconfig.json]
    D --> D2[OAuth2AuthenticationService]
    D --> D3[PermissionService]
    D --> D4[PersistenceService]
    D --> D5[LocaleSwitcher]
    D --> D6{Online?}

    D6 -->|Ja| D7[UGSService]
    D6 -->|Nein| D8[IsOffline = true]

    D7 --> D9[UIManager]
    D8 --> D9

    D9 --> D10[HmdPresenceMonitorService]
    D10 --> D11[CollaborationService]
    D11 --> D12[CommandBusService]

    D12 --> E[OnServicesReady Event]
    E --> F[AppController.HandleServicesInitializedAsync]

    F --> G[Load Intro Scene]
    G --> H[Preload Hamburg + Multiplayer]
    H --> I[Wait 3 Sekunden]
    I --> J[Activate Hamburg Scene]

    J --> K[UIManager.InitializeRootAsync]
    K --> L[Show MenuBar, ToolBar, Disclaimer]
    L --> M[Check HMD Presence]
    M --> N{Headset?}

    N -->|Ja| O[WorldSpace UI Mode]
    N -->|Nein| P[Overlay UI Mode]

    O --> Q[Runtime Loop]
    P --> Q
```

---








## 2. UI Architecture (UIToolkit + MVP + UIManager)

```mermaid
flowchart TB
    subgraph UIManager["UIManager"]
        UM[UIManager.cs]
        REG[UI Regions Dictionary]
        PS1[PanelSettings Overlay]
        PS2[PanelSettings WorldSpace]
    end

    subgraph MVP["MVP Pattern"]
        direction TB
        P[Presenter<br/>Logic + Events]
        V[View<br/>UIDocument + Bindings]
        M[Model<br/>Data Loading]
    end

    subgraph UIToolkit["UIToolkit"]
        UID[UIDocument Component]
        UXML[UXML Layout]
        USS[USS Styles]
        VE[VisualElement Tree]
    end

    subgraph Services["Services"]
        SL[ServiceLocator]
        CFG[ConfigurationService]
        LM[LayerManager]
        HTTP[HttpClientWithRetry]
    end

    %% UIManager connections
    UM --> REG
    UM --> PS1
    UM --> PS2

    %% MVP flow
    UM -->|ShowWindowAsync| P
    P --> V
    P --> M
    V --> P

    %% UIToolkit flow
    V --> UID
    UID --> UXML
    UID --> USS
    UXML --> VE
    USS --> VE

    %% Data flow
    M --> SL
    SL --> CFG
    SL --> LM
    M --> HTTP
    HTTP -->|REST API| Backend[(Backend)]

    %% User interaction
    User((User)) -->|Click/Input| VE
    VE -->|Event| V
    V -->|Callback| P
    P -->|Update| M
    M -->|Data| P
    P -->|Bind| V
```





### UI Regions

```mermaid
flowchart LR
    subgraph Screen["Screen Layout"]
        direction TB
        subgraph Header["Header Region"]
            MB[MenuBar]
            UB[UserButton]
        end

        subgraph Main["Main Area"]
            subgraph Sidebar["Sidebar"]
                TB[ToolBar]
            end
            subgraph Content["Content1"]
                MM[MainMenu]
                PL[ProjectLibrary]
                VAR[Variants]
                SET[Settings]
            end
        end

        subgraph Overlay["Overlay Layer"]
            MOD[Modal Dialogs]
            TOAST[Toast Notifications]
        end
    end
```

---






## 3. Backend/Config Flow

```mermaid
sequenceDiagram
    participant U as User/UI
    participant P as Presenter
    participant M as Model
    participant C as ConfigurationService
    participant H as HttpClientWithRetry
    participant B as Backend API

    U->>P: User Action (z.B. Load Projects)
    P->>M: LoadDataAsync()
    M->>C: GetService<ConfigurationService>()
    C-->>M: AppConfig (URLs, Headers)

    M->>M: Build URL from Endpoint
    Note over M: EndpointPath("ProjectList")<br/>→ /project/list

    M->>H: GetAsync(url, headers, token)

    alt Token expired
        H->>H: Refresh Token
    end

    H->>B: GET /api/gateway/project/list

    alt Success
        B-->>H: 200 OK + JSON
        H-->>M: Response Data
        M->>M: Deserialize JSON
        M-->>P: List<Project>
        P->>P: Update View
    else Error
        B-->>H: 401/404/500
        H->>H: Retry with backoff
        H-->>M: Exception
        M-->>P: Error State
        P->>P: Show Error Toast
    end
```

---







## 4. OAuth2 PKCE + Custom Browser Flow

```mermaid
sequenceDiagram
    participant App as Unity App
    participant Auth as OAuth2AuthService
    participant Srv as LocalHttpServer
    participant Browser as System Browser
    participant IDP as Identity Provider
    participant Store as TokenStorage

    App->>Auth: LoginAsync()
    Auth->>Auth: Generate code_verifier + code_challenge
    Auth->>Srv: Start localhost:48152

    Auth->>Browser: Open Auth URL
    Note over Browser: /authorize?<br/>client_id=urn:fhhvr<br/>redirect_uri=localhost:48152/callback<br/>code_challenge=...<br/>scope=openid profile email

    Browser->>IDP: User enters credentials
    IDP-->>Browser: Redirect to callback
    Browser->>Srv: GET /callback?code=AUTH_CODE

    Srv-->>Auth: Authorization Code
    Auth->>Srv: Stop Server

    Auth->>IDP: POST /token
    Note over Auth: code + code_verifier
    IDP-->>Auth: access_token + refresh_token

    Auth->>Store: Save Tokens
    Note over Store: Windows Credential Manager

    Auth-->>App: Login Success
    App->>App: Update PermissionService
```

---






## 5. Cesium Tile Loading Flow

```mermaid
flowchart TD
    subgraph Init["Initialization"]
        A[LayerManager.InitAsync] --> B[Fetch /public/baseLayer/list]
        B --> C[Create BaseLayerCombined objects]
        C --> D[InstantiateAllBaseLayersAsync]
    end

    subgraph Instantiate["Layer Instantiation"]
        D --> E{Layer Type?}
        E -->|Tiles3D| F[Instantiate CesiumCity Prefab]
        E -->|Terrain| G[Instantiate CesiumTerrain Prefab]
        E -->|Imagery| H[Configure UrlTemplateOverlay]

        F --> I[Find Cesium3DTileset Component]
        G --> I
        I --> J[SetTilesetUrl via Reflection]
    end

    subgraph URL["URL Resolution"]
        J --> K{Cached locally?}
        K -->|Ja| L[file:///VRVisData/layerId/tileset.json]
        K -->|Nein| M[https://server/tiles/tileset.json]
        L --> N[Apply URL to Component]
        M --> N
    end

    subgraph Stream["Cesium Native Streaming"]
        N --> O[Cesium fetches tileset.json]
        O --> P[Parse root tile bounding volume]
        P --> Q[Queue visible tiles based on camera]
        Q --> R[Download tile content async]
        R --> S[Render at LOD based on screenSpaceError]
        S --> T{More tiles needed?}
        T -->|Ja| Q
        T -->|Nein| U[TilesLoaded Event]
    end

    subgraph Progress["Progress Monitoring"]
        Q --> V[CesiumTilesLoadingProgressProvider]
        V --> W[ComputeLoadProgress per tileset]
        W --> X[Average across all active]
        X --> Y[TilesLoadingProgressChanged Event]
    end
```

---






## 6. XR Input Mode Switching

```mermaid
stateDiagram-v2
    [*] --> CheckHMD: App Start

    CheckHMD --> ControllerMode: HMD Present
    CheckHMD --> MouseMode: No HMD

    state ControllerMode {
        [*] --> XRActive
        XRActive --> VRInput: Process XR Input
        VRInput --> XRRaycast: Teleport via Ray
        VRInput --> ContinuousMove: Analog Stick Move
    }

    state MouseMode {
        [*] --> DesktopActive
        DesktopActive --> KeyboardInput: WASD Movement
        KeyboardInput --> MouseLook: Right Click Look
        KeyboardInput --> CtrlTeleport: Ctrl+Mouse Teleport
    }

    ControllerMode --> MouseMode: HMD Disconnected
    MouseMode --> ControllerMode: HMD Connected

    note right of CheckHMD
        HmdPresenceMonitorService
        polls XRNode.Head
    end note
```

---






## 7. Collaboration / Command Flow

```mermaid
sequenceDiagram
    participant Local as Local User
    participant CS as CollaborationService
    participant CB as CommandBusService
    participant VH as VivoxHandler
    participant VC as Vivox Cloud
    participant Remote as Remote Users

    Note over Local,Remote: User joins Meeting

    Local->>CS: CollaborateAsync(meetingEvent)
    CS->>VH: Join Main Channel (audio)
    CS->>VH: Join Command Channel (text)
    VH->>VC: Connect to Vivox

    Note over Local,Remote: Local User sends command

    Local->>CS: Change Variant
    CS->>CB: EnqueueVariant(variantId)

    Note over CB: Rate Limiting Check
    CB->>CB: Throttle 2 seconds

    CB->>VH: SendCommandMessageAsync
    Note over VH: Coalesce duplicate types
    VH->>VC: Text Message (JSON)

    VC->>Remote: Broadcast to channel

    Note over Remote: Remote receives command

    Remote->>VH: OnChannelMessageReceived
    VH->>CB: EnqueueVariant (from remote)
    CB->>CB: Apply after throttle
    CB->>CS: VariantChanged Event
    CS->>CS: LayerManager.SetVariantAsync
```

---




## 8. Service Lifecycle

```mermaid
flowchart TB
    subgraph Registration["Service Registration"]
        A[ServiceLocator.RegisterServicesAsync]
        A --> B{Implements?}
        B -->|IAppServiceAsync| C[await InitServiceAsync]
        B -->|IAppService| D[InitService]
        C --> E[Add to _services Dictionary]
        D --> E
    end

    subgraph Runtime["Runtime Access"]
        F[Any Component]
        F --> G[ServiceLocator.GetService<T>]
        G --> H[Lookup in _services]
        H --> I[Return Service Instance]
    end

    subgraph Teardown["Teardown"]
        J[OnDestroy / App Quit]
        J --> K[UnregisterAllServicesAsync]
        K --> L{For each service}
        L -->|IAppServiceAsync| M[await DisposeServiceAsync]
        L -->|IAppService| N[DisposeService]
        M --> O[Remove from Dictionary]
        N --> O
    end

    E -.-> F
    I -.-> J
```






---
---



# Project Inventory / New-Dev Orientation

## Repo Map: "Where to Find X"

| Was suche ich? | Wo finde ich es? |
|----------------|------------------|
| **Startup-Szene** | `Assets/FHH/Scenes/Startup.unity` |
| **ServiceLocator** | `Assets/FHH/Logic/ServiceLocator.cs` |
| **AppController** | `Assets/FHH/Logic/AppController.cs` |
| **UIManager** | `Assets/FHH/UI/UIManager.cs` |
| **Server-Konfiguration** | `Assets/StreamingAssets/appconfig.json` |
| **AppConfig-Klasse** | `Assets/Foxbyte/Core/Services/ConfigurationService/AppConfig.cs` |
| **LayerManager** | `Assets/FHH/Logic/LayerManager.cs` |
| **PlayerController/Input** | `Assets/FHH/Input/PlayerController.cs` |
| **Input Actions** | `Assets/FHH/Input/XRI VRVis Input Actions.inputactions` |
| **MVP UI-Framework** | `Assets/Foxbyte/Presentation/` |
| **UI Feature Slices** | `Assets/FHH/UI/<FeatureName>/` |
| **OpenXR Settings** | `Assets/XR/Settings/OpenXR Package Settings.asset` |
| **Cesium Prefabs** | `Assets/FHH/Prefabs/` (CesiumCity, CesiumTerrain, CesiumTrees) |
| **Build Profile** | `Assets/Settings/Build Profiles/Windows Release.asset` |
| **Localization Tables** | `Assets/FHH/Localization/Tables/` |
| **User Settings Model** | `Assets/FHH/Logic/Models/UserSettings.cs` |
| **Networking/Collaboration** | `Assets/FHH/Logic/Components/Networking/` |
| **VR Locomotion** | `Assets/FHH/Logic/VR/` |








---








## Key Folders and Their Contents

### `Assets/FHH/` — Domain-Specific Code
```
Assets/FHH/
├── Scenes/              → Startup, Intro, Hamburg, UI, Multiplayer
├── Logic/               → ServiceLocator, AppController, LayerManager
│   ├── Components/      → Cesium, Networking, Collaboration, HmdPresence
│   ├── Models/          → Project, User, UserSettings, DTOs
│   └── VR/              → Teleportation, Origin alignment
├── UI/                  → MVP Feature Slices (MenuBar, ToolBar, etc.)
├── Input/               → PlayerController, Input Actions
├── Prefabs/             → ServiceLocator, XR Rig, UI, Cesium templates
├── Localization/        → String tables (de, en)
└── AppResources/        → Materials, Fonts, Textures
```

### `Assets/Foxbyte/` — Reusable Framework
```
Assets/Foxbyte/
├── Core/
│   ├── ServiceLocator/  → ServiceLocatorBase, IAppService interfaces
│   ├── Services/
│   │   ├── ConfigurationService/  → AppConfig loading, user prefs
│   │   ├── OAuth/                 → OAuth2 flow, token storage
│   │   ├── Permission/            → Role-based access (Owner/Mod/User)
│   │   └── Persistence/           → JSON persistence, PlayerPrefs
│   └── Http/            → HttpClientWithRetry (circuit breaker, backoff)
└── Presentation/
    ├── PresenterBase.cs → MVP Presenter base class
    ├── ViewBase.cs      → MVP View base class
    ├── UIProjectContext.cs → UI Regions (Header, Sidebar, Content, etc.)
    └── WindowOptions.cs → Window display configuration
```

### `Assets/StreamingAssets/`
```
appconfig.json          → Server URLs, OAuth, TilesBaseUrl, endpoints
```

### `Assets/XR/Settings/`
```
OpenXR Package Settings.asset → Controller profiles, render mode
```

---

## Entry Points (Scenes, Bootstraps)

### Boot Sequence
```
1. Startup.unity lädt
   └─> ServiceLocator.Awake() → DontDestroyOnLoad
       └─> RegisterServicesAsync()
           ├─> ConfigurationService (lädt appconfig.json)
           ├─> OAuth2AuthenticationService
           ├─> PermissionService
           ├─> PersistenceService
           ├─> LocaleSwitcher
           ├─> UGSService (optional, wenn online)
           ├─> UIManager
           ├─> HmdPresenceMonitorService (optional)
           ├─> CollaborationService
           └─> CommandBusService

2. AppController.OnServicesReady()
   └─> Lädt Intro-Szene (3s anzeigen)
   └─> Preload Hamburg + Multiplayer Szenen
   └─> Aktiviert Hamburg-Szene
   └─> Initialisiert UIManager mit Regionen
   └─> Zeigt MenuBar, ToolBar, Disclaimer

3. Runtime Loop
   └─> Input verarbeiten (VR oder Desktop)
   └─> Cesium Tiles streamen
   └─> UI Events verarbeiten
```

### Scene Overview
| Szene | Zweck |
|-------|-------|
| `Startup.unity` | Bootstrap, ServiceLocator |
| `Intro.unity` | Splash Screen (3 Sekunden) |
| `Hamburg.unity` | Hauptszene mit Cesium-Daten |
| `UI.unity` | UI Root Layer |
| `Multiplayer.unity` | Collaboration/Networking |

---

## Important Concepts

### 1. ServiceLocator Pattern
- Zentrale Service-Registry für alle Dienste
- Zugriff: `ServiceLocator.GetService<T>()`
- Services implementieren `IAppService` oder `IAppServiceAsync`

### 2. MVP UI Architecture
- **Model**: Daten laden via Services
- **View**: UIDocument + UXML/USS bindings
- **Presenter**: Logik, Event-Subscriptions
- Location: `Assets/FHH/UI/<Feature>/`

### 3. appconfig.json
- Externalisierte Konfiguration
- Server-URLs, OAuth, TilesBaseUrl
- Keine Code-Änderungen für City-Wechsel

### 4. LayerManager
- Orchestriert Cesium 3D Tiles
- Verwaltet Projekte und Varianten
- Events: `OnProjectChanged`, `OnVariantChanged`

### 5. UniTask Async Pattern
- Ersetzt `Task/await` für bessere Performance
- `await UniTask.Delay()`, `.Forget()` für fire-and-forget
- CancellationToken Support überall

### 6. XR Fallback System
- `HmdPresenceMonitorService` erkennt Headset
- `PlayerController` wechselt zwischen Controller/Mouse
- Automatic: VR-Headset anschließen → VR-Modus

### 7. UIManager Window Lifecycle
- `ShowWindowAsync<P,V,M>()` zeigt Presenter
- `HideAsync<P>()` schließt Fenster
- Regions: Header, Sidebar, Content1, Overlay, Toast

### 8. OAuth2 + Custom Browser Flow
- Loopback redirect: `localhost:48152/callback`
- Token in Windows Credential Manager
- Automatic refresh

### 9. Vivox Voice + Command Bus
- Text-Channel für Commands (Variant, Teleport, Sun)
- Audio-Channel für Voice
- Rate limiting (2s throttle)

### 10. Offline Mode
- `ServiceLocator.IsOffline` Flag
- Cached Layers funktionieren offline
- UGS/Collaboration deaktiviert

---

## Quick Reference: Adding Features

### Neues UI-Fenster
```
1. Ordner erstellen: Assets/FHH/UI/<FeatureName>/
2. Dateien:
   - <Feature>Presenter.cs : PresenterBase<P,V,M>
   - <Feature>View.cs : ViewBase<P>
   - <Feature>Model.cs : PresenterModelBase
3. Anzeigen via: UIManager.ShowWindowAsync<P,V,M>()
```

### Neuer Backend-Endpoint
```
1. appconfig.json: Endpoint hinzufügen
2. ConfigurationService: URL abrufen
3. HttpClientWithRetry: Request senden
```

### Neue Config-Eigenschaft
```
1. appconfig.json: Wert hinzufügen
2. AppConfig.cs: Property hinzufügen
3. Zugriff: ServiceLocator.GetService<ConfigurationService>().AppConfig
```

