# PublicDomain-Internet-Library
C# ASP.NET-ben írt, internetes könyvár megvalósítása. 

A project egy internetes könyvtárat valósít meg, ami szabadon terjeszthető könyveket tesz elérhetővé az oktatás segítésére. Az alapötletet a Magyar Elektronikus Könyvtár (www.mek.oszk.hu) és a Project Gutenberg (www.gutenberg.org) adta.

## Hogyan működik?
A főoldalon könyveket lehet keresni író, cím, vagy ezek kombinációja alapján.
A bejelentkezett felhasználók ezeket le is tudják tölteni.
A könyvtárosok könyveket tölthetnek fel, módosíthatnak vagy törölhetnek.
Az adminisztrátor pedig megadhatja, vagy elveheti a könyvtárosi jogot bárkitől.

## Adminisztrátor
A program elindulásakor automatikusan létrehoz egy adminisztrátort a következő adatokkal:
        Email: admin@admin                 
        Jelszó: Jelszó1

## Könyvtáros létrehozása
1) Regisztráció normál felhasználóként.
2) Ezután az adminisztrátor a Felhasználók menüpont alatt megadhatja neki a könyvtárosi jogot.
3) Könyvtárosként a főoldalon már nem csak keresni és letölteni tud, hanem feltölteni, módosítani és törölni is.

## Felhasználó
Bejelentkezett felhasználó le tudja tölteni a kívánt könyvet.

## Regisztrálatlan felhasználó
Tud keresni, és ha megtalálta a kívánt könyvet regisztrálhat, vagy bejelentkezhet.


## Hátralévő, még megvalósítandó tennivalók
- Refaktorálás
- Elgépelések javítása

    
## A program futtatása (Visual studio-ban)
  1) Github repó klónozása a helyi gépre
  2) appsettings.json-ban átírni a ConnectionString-et, hogy a használt SQL szerverre mutasson
  3) Üres adatbázis létrehozása Package Manager Console-ban: 
        add-migration InitialMigration
        update-database
  4) Alapesetben csak az admin felhasználó létezik
        Email: admin@admin                 
        Jelszó: Jelszó1
  5) Ezután már lehet könyveket feltölteni (admin, vagy könyvtáros tud)