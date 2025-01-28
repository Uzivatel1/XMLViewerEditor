### ⚠️ Důležité: Možný problém při stažení jako ZIP
Pokud stáhnete tento projekt jako **ZIP soubor**, může se stát, že nepůjde sestavit kvůli chybě `MSB3821` u `Form1.resx`.  
Tento problém se týká všech souborů stažených z internetu.  

#### ✅ Řešení:
1. **Před rozbalením ZIP odblokujte:**  
   - Klikněte pravým tlačítkem na stažený soubor → **Vlastnosti** → **Odblokovat** → **Použít** → **OK**.  
   - Poté rozbalte a otevřete projekt.  

2. **Doporučeno: Použijte `git clone` místo stahování ZIPu:**  
   ```powershell
   git clone https://github.com/tvoje-jmeno/tvuj-repozitar.git
