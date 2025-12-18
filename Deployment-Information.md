**Konten & Unity-Projekt (Unity 6.2)**
- falls noch nicht vorhanden: Unity-ID + eigene Unity-Organisation anlegen, neues Projekt im Unity Dashboard erstellen: [https://cloud.unity.com](https://cloud.unity.com)
- Unity 6.2 Handbuch: [https://docs.unity3d.com/6000.2/Documentation/Manual/UnityManual.html](https://docs.unity3d.com/6000.2/Documentation/Manual/UnityManual.html?utm_source=chatgpt.com)

**UGS-Dienste aktivieren (inkl. Anonymous Sign-In)**
- Im Dashboard für dieses Projekt aktivieren:
 - **Authentication** (erforderlich für anonymen Login). Einrichtung eines speziellen Logins (Unity Authentication/Anschluss IDP) ist nicht notwendig, da nicht genutzt.
 - **Vivox Voice & Text**

**Project ID – Herkunft und Verwendung (Unity 6.2)**
- Beim Anlegen eines Projekts im Unity Dashboard wird automatisch eine **Unity Project ID** erzeugt (eindeutige Online-Kennung des Projekts)
- Im Editor (zum Zeitpunkt der Übergabe Unity 6000.2.7f2):
 - Projekt öffnen, mit **eigenem** Unity-Account anmelden
 - Edit → Project Settings → **Services**: hier sieht man Organisation, Projektname und **Unity Project ID**; diese wird automatisch gesetzt, wenn man den Editor mit dem Dashboard-Projekt verknüpft. Man muss die ID in der Regel nicht manuell eintragen
 - Edit → Project Settings → **Services** → Vivox: Sicherstellen, dass die in der Cloud hinterlegten Credentials genutzt werden

**Netzwerk / Firewall – Unity-Dienste allgemein (UGS)**
- Von Client-Rechnern ausgehend **TCP 443 (HTTPS)** ins Internet erlauben, insbesondere zu Unity-/UGS-Domains (Dashboard, Auth, Vivox-Signalisierung). Keine eingehenden Ports nötig
- Feste IP-Adressen für UGS werden nicht garantiert; Unity nutzt dynamische Cloud-Infrastruktur. Falls die Umgebung IP-Whitelists erzwingt, sollte wenn möglich per **Domain/URL-Whitelist** gearbeitet werden, nicht per statischer IP-Liste

**Netzwerk / Firewall – Vivox Voice & Text (Ports/IPs)**
- Offizielle, aktuelle Liste der **erforderlichen IP-Bereiche und Ports** immer hier prüfen (an IT weitergeben):
 - „Vivox: What IPs and ports are required for Vivox to work?“  
  https://support.unity.com/hc/en-us/articles/4407491745940

**Windows-Client-Firewall / Endpoint-Security**
- Sicherstellen, dass die Applikations-EXE auf dem Client ausgehend **TCP 443** und die von IT freigegebenen **Vivox-UDP-Ports** nutzen darf (siehe Vivox-Firewall-Artikel)

**Lokaler Callback-Server für Custom-IDP (`http://localhost:48152/callback`)**
- Die App startet lokal auf dem Client einen kleinen HTTP-Listener auf **127.0.0.1:48152** und erhält dort die Tokens nach erfolgreichem Login über den Browser-Redirect
- Dafür ist im Unternehmensnetz üblicherweise **keine** Änderung an der Perimeter-Firewall nötig, aber auf dem Client selbst:
 - Lokale OS-Firewall (z. B. Windows Defender Firewall) muss der Spiele-EXE **eingehende Verbindungen auf `127.0.0.1:48152` über TCP** erlauben (Loopback-Verkehr)
 - Endpoint-Security-/„Zero Trust“-Agenten dürfen **lokale `http://localhost:48152/...`-Aufrufe nicht blockieren oder umleiten**
 - Browser-Gruppenrichtlinien dürfen `http://localhost` nicht sperren bzw. müssen Ausnahmen für `http://localhost:48152/callback` zulassen

 **Weiteres**: 
 - Relay wird nicht genutzt. Damit verbundene Freigaben sind nicht notwendig
 - OpenXR Runtime muss installiert und aktiviert / als Standard gesetzt sein. 
   **Vor dem Starten der App** wird empfohlen, die jeweilige Hersteller-Software für den Headset-Link zu starten und die Controller zu initialisieren.
 