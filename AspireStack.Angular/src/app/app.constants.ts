export class AppConstants {
    static readonly API_ENDPOINT: string = 'https://api.example.com';
    static readonly APP_TITLE: string = 'AspireStack';
    static readonly DEFAULT_LANGUAGE: string = 'en-US';
    static readonly SUPPORTED_LANGUAGES: string[] = ["en-US", "fr-FR", "tr-TR", "ru-RU", "az-AZ"];
    static readonly DATE_FORMAT: string = 'YYYY-MM-DD';
    static readonly TIME_FORMAT: string = 'HH:mm:ss';
    static readonly AUTH_TOKEN_COOKIE_NAME: string = 'authToken';
    static readonly LANGUAGE_COOKIE_NAME: string = 'language';
    static readonly AcceptLanguageHeader: string = 'Accept-Language';
    static readonly AuthorizationHeader: string = 'Authorization';
}