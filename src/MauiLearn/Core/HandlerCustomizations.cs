using MauiLearn.Controls;

namespace MauiLearn.Core;

/// <summary>
/// Handlers ligam o controle multiplataforma (ex.: Entry) ao controle nativo
/// (TextBox no Windows, UITextField no Mac Catalyst). O "Mapper" de cada handler
/// diz como cada propriedade é aplicada na view nativa — e podemos estendê-lo.
/// </summary>
public static class HandlerCustomizations
{
    public static void Apply()
    {
        // AppendToMapping: roda DEPOIS do mapeamento padrão da propriedade/chave.
        // Aqui criamos uma chave nova ("Borderless") que só afeta BorderlessEntry.
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
        {
            if (view is not BorderlessEntry)
                return;

#if WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = null;
#elif MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
#endif
        });
    }
}
