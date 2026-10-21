# MauiLearn — a biblioteca interativa do .NET MAUI 11, feita em .NET MAUI

Um app de **desktop (Windows e macOS)** para aprender .NET MAUI de verdade: cada componente tem uma
**prévia ao vivo**, um **inspetor com TODAS as propriedades** (editáveis), um **espião de eventos**
e o **XAML equivalente** gerado na hora. Além disso: **21 aulas** com exemplos interativos e quiz, e
**14 desafios** para resolver editando XAML com **Hot Reload**, com verificação automática.

> O próprio código do app é material de estudo: todos os `.xaml` e `.cs` são comentados em português.

---

## ✨ O que tem dentro

| Área | O que você faz |
| --- | --- |
| 🧩 **Catálogo** (72 componentes) | Layouts, texto, entrada, botões, coleções, formas/pincéis, gestos, páginas e recursos de desktop. Para cada um: prévia, inspetor de propriedades por reflexão (agrupadas pela classe que as declara: `Button → View → VisualElement → …`), XAML gerado das suas alterações, eventos ao vivo, propriedades-chave, nuances e exemplos de código. |
| 📚 **Aulas** (21) | Do “o que é o MAUI” até handlers, controles customizados e novidades do .NET MAUI 10/11 — com laboratórios interativos (Margin × Padding, Grid, modos de binding, animações, gradientes, sombras, AdaptiveTrigger…) e quiz no final. |
| 🏆 **Desafios** (14) | Arquivos XAML em `src/MauiLearn/Challenges/Workspaces/`. Com o app rodando, edite, salve, veja a mudança via Hot Reload e clique em **Verificar**. Dicas progressivas e solução. |
| 🧪 **Playground XAML** | Digite XAML e veja o resultado na hora (parser de runtime), com exemplos prontos. |
| ⚙️ **Configurações** | Tema claro/escuro, múltiplas janelas, informações do ambiente e progresso. |

Recursos de desktop usados pelo app: **MenuBar** com atalhos (Ctrl/⌘+1…5 navegam, Ctrl/⌘+K busca,
Ctrl/⌘+Shift+N abre nova janela), **menus de contexto** (botão direito), **tooltips**, **hover**
(`PointerOver`), **múltiplas janelas**, **TitleBar** customizada (Windows) e **badges** no Shell.

---

## 🧰 Requisitos

- **.NET 11 SDK** (RC2 ou mais recente) — o `global.json` aceita `11.0.100-rc.2` em diante.
- **Workload do MAUI**: `dotnet workload install maui`
- **macOS**: Xcode na versão exigida pelo .NET 11 (instale o mais recente da App Store) para compilar `net11.0-maccatalyst`.
- **Windows**: Windows 10 (1809+) ou 11 para `net11.0-windows10.0.19041.0`.
- IDE (qualquer uma):
  - **Visual Studio 2026** (Windows) com a carga de trabalho *.NET Multi-platform App UI*;
  - **VS Code** com as extensões *.NET MAUI* e *C# Dev Kit* (Windows e macOS) — recomendadas em `.vscode/extensions.json`;
  - **JetBrains Rider**.

## ▶️ Como rodar

```bash
git clone https://github.com/ArturChaves/MauiLearn.git
cd MauiLearn
dotnet workload install maui     # uma vez

# macOS (Mac Catalyst)
dotnet build -t:Run -f net11.0-maccatalyst src/MauiLearn/MauiLearn.csproj

# Windows
dotnet build -t:Run -f net11.0-windows10.0.19041.0 src/MauiLearn/MauiLearn.csproj
```

Para **Hot Reload**, rode pela IDE em **Debug** (F5). Veja a **Aula 03** dentro do app.

> O alvo Windows só é incluído no `TargetFrameworks` quando você compila no Windows; no Mac, o projeto
> compila apenas para Mac Catalyst.

---

## 🗺️ Trilha de aulas

| # | Aula | Nível |
| --- | --- | --- |
| 01 | O que é o .NET MAUI (arquitetura e handlers — veja o tipo NATIVO de cada controle) | Básico |
| 02 | Anatomia de um projeto MAUI | Básico |
| 03 | Hot Reload e fluxo de trabalho | Básico |
| 04 | XAML essencial (type converters, property elements, markup extensions, xmlns global) | Básico |
| 05 | Layouts (laboratório de Margin/Padding/Options e troca de layouts) | Básico |
| 06 | Grid a fundo (laboratório de linhas/colunas) | Básico |
| 07 | Data Binding (modos, converters, MultiBinding, RelativeSource, compiled bindings) | Intermediário |
| 08 | MVVM (INotifyPropertyChanged, ICommand, ObservableCollection, CommunityToolkit) | Intermediário |
| 09 | Recursos, estilos e temas (Static × DynamicResource, BasedOn, StyleClass, AppThemeBinding) | Intermediário |
| 10 | Navegação (Shell, rotas, IQueryAttributable, modais, diálogos) | Intermediário |
| 11 | Coleções (CollectionView, DataTemplateSelector, agrupamento, rolagem infinita) | Intermediário |
| 12 | Triggers, Behaviors e VisualStateManager (inclui AdaptiveTrigger) | Intermediário |
| 13 | Gestos e entrada no desktop (inclui o novo LongPress) | Intermediário |
| 14 | Animações (*ToAsync, Easing, Animation composta) | Intermediário |
| 15 | Formas, pincéis, sombras e GraphicsView | Intermediário |
| 16 | Recursos de desktop (MenuBar, atalhos, contexto, janelas, TitleBar) | Intermediário |
| 17 | Handlers e código por plataforma | Avançado |
| 18 | Ciclo de vida (log ao vivo) | Avançado |
| 19 | APIs de plataforma (Preferences, SecureStorage, FilePicker, TTS, Screenshot…) | Intermediário |
| 20 | Criando seus próprios controles (BindableProperty, ControlTemplate) | Avançado |
| 21 | Novidades do .NET MAUI 10 e 11 (SourceGen, xmlns global, C# Expressions, obsoletos) | Intermediário |

## 🏆 Desafios

| # | Desafio | Arquivo |
| --- | --- | --- |
| 01 | Olá, Hot Reload! | `Challenges/Workspaces/D01OlaHotReload.xaml` |
| 02 | Botão estiloso | `D02BotaoEstiloso.xaml` |
| 03 | Lado a lado | `D03LadoALado.xaml` |
| 04 | Centralizado de verdade | `D04Centralizado.xaml` |
| 05 | Jogo da velha (Grid 3×3) | `D05Grade.xaml` |
| 06 | Ligando um Slider (binding) | `D06Binding.xaml` |
| 07 | Estilo reutilizável | `D07Estilo.xaml` |
| 08 | Botão que só habilita com texto (DataTrigger) | `D08Trigger.xaml` |
| 09 | Lista em grade (CollectionView) | `D09ListaGrade.xaml` |
| 10 | Hover no desktop (VisualStateManager) | `D10Hover.xaml` |
| 11 | Cantos e sombra | `D11Sombra.xaml` |
| 12 | MVVM: ligue o contador | `D12Mvvm.xaml` |
| 13 | Duplo clique (gestos) | `D13DuploClique.xaml` |
| 14 | Layout responsivo (AdaptiveTrigger) | `D14Responsivo.xaml` |

Cada arquivo começa com um comentário explicando o objetivo. Errou tudo? `git checkout -- src/MauiLearn/Challenges/Workspaces/`
restaura os originais.

---

## 🗂️ Estrutura

```
src/MauiLearn/
  MauiProgram.cs, App.xaml(.cs), AppShell.xaml(.cs), GlobalXmlns.cs
  Catalog/        ← o catálogo: ComponentCatalog.<Categoria>.cs (API fluente) + modelos
  Controls/       ← PropertyInspectorView, EventLogView, CodeBlock, TipCard, AppPage (MenuBar), RatingView…
  Core/           ← reflexão de BindableProperty, ProgressService, ObservableObject, RelayCommand, handlers
  Pages/          ← Início, Catálogo, Detalhe do componente, Aulas, Desafios, Playground, Configurações
  Lessons/        ← LessonCatalog.cs + L01…L21 (ContentView XAML + code-behind)
  Challenges/     ← ChallengeCatalog.cs (verificações) + Workspaces/ (os XAML que você edita)
  ViewModels/  Converters/  Resources/  Platforms/{MacCatalyst,Windows}
tests/MauiLearn.Smoke/  ← teste de fumaça (roda em qualquer SO, sem Windows/Mac)
```

## 🔧 Estendendo (ótimos exercícios!)

**Adicionar um componente ao catálogo** — em `Catalog/ComponentCatalog.<Categoria>.cs`:

```csharp
C(list, "MeuControle", cat, "⭐", "Resumo em uma linha.")
    .About("Descrição longa.")
    .Demo(() => new MeuControle { /* valores iniciais */ })
    .Prop("Propriedade", "O que ela faz.")
    .Nuance("Um detalhe que pega muita gente.")
    .Sample("""<MeuControle Propriedade="..." />""");
```

O inspetor descobre todas as `BindableProperty` sozinho, por reflexão.

**Adicionar um desafio** — crie um `ContentView` em `Challenges/Workspaces/` e registre-o em
`Challenges/ChallengeCatalog.cs` com `.Check(...)` que inspeciona a área de trabalho ao vivo.

## ✅ Teste de fumaça

```bash
cd tests/MauiLearn.Smoke
dotnet run
```

Compila o código compartilhado para .NET 11 “puro” e instancia todas as aulas, páginas, demos do
catálogo (com inspetor e espião de eventos), desafios — e confere que **cada solução de desafio passa
nas próprias verificações**. Rode-o depois de mudar o catálogo ou os desafios.

## 📝 Notas

- O projeto silencia `CS0618` (APIs obsoletas) só para poder **ensinar** `ListView`, `TableView`, `Frame` e as Cells. No seu código, deixe o aviso ligado.
- XAML C# Expressions (`{= ...}`) são experimentais no .NET MAUI 11: descomente `<EnablePreviewFeatures>` no `.csproj` para testar.
- O modo de Hot Reload `SourceGen` (incremental) é opcional: descomente `<MauiXamlHotReload>` no `.csproj`.
- Mac Catalyst: `UIDeviceFamily = 2` (“Optimize for Mac”), App Sandbox + acesso à rede em `Entitlements.plist`, e múltiplas janelas via `SceneDelegate`.
