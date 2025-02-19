import { Constants } from './constants';
import { CurrentUserService } from './app/services/current-user.service';
import { LocalizationService } from './app/services/localization/localization.service';

/**
 * Fetches all permissions from the server and sets them in the CurrentUserService.
 * 
 * This function makes a synchronous HTTP GET request to the '/api/Role/GetAllPermissions' endpoint.
 * It includes an authorization header with a bearer token retrieved from local storage.
 * 
 * If the request is successful (status 200), the response data is parsed as JSON and passed to the 
 * `CurrentUserService.setAllPermissions` method. If the request fails, an error message is logged to the console.
 * 
 * @throws Will log an error message to the console if the request fails.
 */
export const getAllPermissions = () => {
  const xhr = new XMLHttpRequest();
  xhr.open('GET', '/api/Role/GetAllPermissions', false); // false makes the request synchronous
  xhr.setRequestHeader('Authorization', 'Bearer ' + localStorage.getItem(Constants.AUTH_TOKEN_COOKIE_NAME));
  xhr.onreadystatechange = () => {
    if (xhr.readyState === 4) {
      if (xhr.status === 200) {
        const data = JSON.parse(xhr.responseText);
        CurrentUserService.setAllPermissions(data);
      } else {
        console.error('Error getAllPermissions', xhr.statusText);
      }
    }
  };
  xhr.send();
};


/**
 * Fetches the current localization translations synchronously and sets them in the LocalizationService.
 * 
 * This function makes a synchronous XMLHttpRequest to the endpoint '/api/Localization/GetCurrentLocalization'
 * to retrieve the current localization data. It sets the 'Accept-Language' header to the current language
 * obtained from the LocalizationService. Upon successful response, it parses the JSON response and updates
 * the LocalizationService with the retrieved translations and supported cultures.
 * 
 * @remarks
 * Synchronous XMLHttpRequests are generally discouraged as they can block the main thread, negatively impacting
 * the user experience. Consider using asynchronous requests with Promises or async/await for better performance.
 * 
 * @example
 * gettingTranslations();
 * 
 * @throws Will log an error message to the console if the request fails.
 */
export const gettingTranslations = () => {
  const xhr = new XMLHttpRequest();
  xhr.open('GET', '/api/Localization/GetCurrentLocalization', false); // false makes the request synchronous
  xhr.setRequestHeader('Accept-Language', LocalizationService.getCurrentLanguage());
  xhr.onreadystatechange = () => {
    if (xhr.readyState === 4) {
      if (xhr.status === 200) {
        const data = JSON.parse(xhr.responseText);
        LocalizationService.setTranslations(data.resources, data.supportedCultures);
      } else {
        console.error('Error Getting translations', xhr.statusText);
      }
    }
  };
  xhr.send();
};

