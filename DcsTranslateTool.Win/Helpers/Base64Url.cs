namespace DcsTranslateTool.Win.Helpers;

public static class Base64Url {
    public static string Encode( byte[] data ) =>
        Convert.ToBase64String( data )
        .TrimEnd( '=' )
        .Replace( '+', '-' )
        .Replace( '/', '_' );

    public static byte[] Decode( string input ) {
        var padded = input
            .Replace( '-', '+' )
            .Replace( '_', '/' );
        switch(padded.Length % 4) {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        return Convert.FromBase64String( padded );
    }

}
