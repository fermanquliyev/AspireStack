export class WebApiResult<T> {
    data?: T;
    success: boolean = false;
    message?: string;
    statusCode: number = 0;
}