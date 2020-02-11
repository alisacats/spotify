import { StartAccountPlayingResponse, StopAccountPlayingResponse, AccountPlaylist, SyncAccountPlaylistRequest, SyncAccountPlaylistResponse, StartAccountPlayingRequest, StopAccountPlayingRequest, GetAccountStatisticsResponse, GetAccountStateResponse, GetAccountPlaylistResponse } from './models';
import { RestClient } from "./rest-client";

export class AccountService {
  constructor(private readonly restClient: RestClient, private readonly accountId: number) { }

  private getUrl(urlSuffix: string) {
    return `/accounts/${this.accountId}/${urlSuffix}`;
  }

  private get<T>(urlSuffix: string) {
    return this.restClient.get<T>(this.getUrl(urlSuffix));
  }

  private post<T>(urlSuffix: string, body: any) {
    return this.restClient.post<T>(this.getUrl(urlSuffix), body);
  }

  async getState() {
    return await this.get<GetAccountStateResponse>('get-state');
  }

  async startPlaying() {
    const req: StartAccountPlayingRequest = {};
    return await this.post<StartAccountPlayingResponse>('start-playing', req);
  }

  async stopPlaying() {
    const req: StopAccountPlayingRequest = {};
    return await this.post<StopAccountPlayingResponse>('stop-playing', req);
  }

  async getPlaylist() {
    return await this.get<GetAccountPlaylistResponse>('get-playlist');
  }

  async syncPlaylist(playlist: AccountPlaylist) {
    const req: SyncAccountPlaylistRequest = { playlist };
    return await this.post<SyncAccountPlaylistResponse>('sync-playlist', req);
  }
  
  async getStatistics() {
    return await this.get<GetAccountStatisticsResponse>('get-statistics');
  }
}
