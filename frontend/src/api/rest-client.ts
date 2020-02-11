import { HttpClient, json } from "aurelia-fetch-client";
import { autoinject } from "aurelia-framework";

@autoinject
export class RestClient {
  constructor(private readonly httpClient: HttpClient) { }

  async get<T>(url: string) {
    const resp = await this.httpClient.fetch(url, { method: 'get' });
    return await resp.json() as T;
  }
  
  async post<T>(url: string, body: any) {
    const resp = await this.httpClient.fetch(url, { method: 'post', body: json(body) });
    return await resp.json() as T;
  }
}
