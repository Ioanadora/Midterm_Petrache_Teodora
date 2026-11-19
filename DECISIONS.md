# DECISIONS

## 1. A rule you moved

Am ales regula R7, care verifica daca doua rezervari se suprapun. In proiectul initial, verificarea era in `Services/BookingService.cs`, in metoda `Create`, unde se folosea si o conditie gresita pentru intervale. Acum regula este in `Domain/Bookings/Booking.cs`, in metoda `OverlapsWith`, si foloseste si `DateRange.Overlaps`. Am mutat-o acolo deoarece verificarea tine de rezervare si nu ar trebui sa fie facuta direct in service sau endpoint.

## 2. Result instead of an exception

`Result<T>` este mai potrivit pentru cazul in care sala este ocupata, deoarece este o situatie normala de business, nu o eroare neasteptata a programului. In `CreateBookingHandler`, daca exista o suprapunere, se returneaza `BookingErrors.RoomOccupied`, care este de tip `Conflict`. Endpointul verifica `result.IsFailure` si foloseste `ErrorHttpExtensions.ToProblem()` pentru a transforma eroarea intr-un raspuns HTTP 409. Asa pot trata erorile fara sa folosesc exceptii pentru fiecare validare.

## 3. Who knows about EF Core?

EF Core este folosit in clasele din `Infrastructure/Persistence`, cum ar fi `RoomHubDbContext`, `RoomRepository`, `BookingRepository` si configurarile pentru entitati. Clasele din `Domain` si handlerele din `Features` nu folosesc direct EF Core, ci lucreaza prin interfete precum `IRoomRepository`, `IBookingRepository` si `IUnitOfWork`. Avantajul este ca logica de business nu depinde direct de baza de date si poate fi testata mai usor. Dezavantajul este ca trebuie sa scriu mai multe interfete si clase, chiar si pentru operatii destul de simple.

## 4. The flow of a request

Cand se trimite un `POST /api/rooms/{roomId}/bookings`, cererea ajunge in `CreateBookingEndpoint`, care creeaza un `CreateBookingCommand` si il trimite la `CreateBookingHandler`. Handlerul cauta camera prin `IRoomRepository`, foloseste `TimeProvider` si apeleaza `Booking.Create` si `DateRange.Create` pentru validari. Apoi verifica limita zilnica R9 prin `CountActiveForParticipantOnDateAsync` si suprapunerile R7 prin `ListActiveForRoomAsync`, folosind metodele din domeniu. Daca totul este valid, rezervarea este adaugata prin `IBookingRepository`, salvata cu `IUnitOfWork.SaveChangesAsync`, iar endpointul returneaza 201.

 
## 5. What you rejected from what the AI gave you

La instalarea EF Core, comanda propusa initial fara o versiune specificata a incercat sa instaleze versiunea 11, care nu era compatibila cu .NET 10. Am corectat problema folosind versiunea `10.0.15` in `RoomHub.Api.csproj`. Tot in timpul testarii a aparut o problema cu Windows Smart App Control si s-au incercat cateva variante de configurare. Pana la urma am pastrat `UseAppHost` pe `false` si nu am pastrat proiectul separat de teste xUnit, ca sa nu complic structura proiectului.

## 6. If you had 30 more minutes

Daca as mai avea 30 de minute, as adauga teste automate pentru regulile de rezervare, mai ales pentru R9, R10 si R11. As folosi `FakeTimeProvider` ca sa pot testa exact limita de 60 de minute pentru anulare si as testa si persistenta cu SQLite in-memory. As inlocui si `EnsureCreated()` cu migrari EF Core, pentru a putea modifica baza de date mai usor pe viitor. Un risc care a ramas este ca doua cereri trimise simultan pot trece ambele de verificarea suprapunerii inainte sa fie salvate, ceea ce poate duce la rezervari in conflict.
