import { AccountState, Track } from "api/models";
import { autoinject, computedFrom } from "aurelia-framework";
import { AccountService } from '../api/account-service';
import { TrackStatistics, AccountPlaylist } from '../api/models';
import { delay } from "bluebird";
import { AccountsService } from "api/accounts-service";

type Tab = 'control' | 'statistics' | 'playlist';

@autoinject
export class Account {
  private _accountService?: AccountService;
  private _reqCounts = 0;
  state: AccountState = { isPlaying: false };
  statistics: TrackStatistics[] = [];
  originalPlaylist: AccountPlaylist = { tracks: [] };
  playlist: AccountPlaylist = { tracks: [] };
  selectedTab: Tab = 'control';
  newTrackId = '';
  newTrackTitle = '';

  @computedFrom('_reqCounts')
  get isBusy() {
    return this._reqCounts > 0;
  }

  selectControlTab() {
    this.selectedTab = 'control';
  }

  selectStatisticsTab() {
    this.selectedTab = 'statistics';
  }

  selectPlaylistTab() {
    this.selectedTab = 'playlist';
  }

  constructor(
    private readonly accountsService: AccountsService
  ) { }

  activate(params: { accountId: number }) {
    this._accountService = this.accountsService.getAccountService(params.accountId);
    this.runFetchLoop();
  }


  async performReq<T>(f: () => Promise<T>) {
    this._reqCounts++;
    try {
      return await f();
    }
    finally {
      this._reqCounts--;
    }
  }


  updatePlaylist() {
    let tracks = this.originalPlaylist.tracks.slice();
    tracks = tracks.filter(x => !this.tracksToRemove.some(y => y.id === x.id));
    tracks.push(...this.tracksToAdd);
    this.playlist = { tracks };
  }

  async fetchData() {
    const stateReq = this._accountService!.getState();
    const statisticsReq = this._accountService!.getStatistics();
    const playlistReq = this._accountService!.getPlaylist();

    this.state = (await stateReq).state;
    this.statistics = (await statisticsReq).statistics;

    this.originalPlaylist = (await playlistReq).playlist;
    this.updatePlaylist();
  }

  async runFetchLoop() {
    while (true) {
      try {
        await this.fetchData();
      }
      catch { }
      await delay(1000);
    }
  }

  async togglePlaying() {
    const req = this.state.isPlaying
      ? this.performReq(() => this._accountService!.stopPlaying())
      : this.performReq(() => this._accountService!.startPlaying());
    this.state = (await req).state;
  }

  tracksToRemove: Track[] = [];
  tracksToAdd: Track[] = [];

  removeTrack(track: Track) {
    if (this.isBusy) return;

    if (this.tracksToAdd.includes(track)) {
      this.tracksToAdd = this.tracksToAdd.filter(x => x !== track);
    }
    else {
      this.tracksToRemove.push(track);
    }
    this.updatePlaylist();
  }

  addTrack() {
    if (this.isBusy) return;

    this.tracksToAdd.push({ id: this.newTrackId, title: this.newTrackTitle });
    this.updatePlaylist();
    
    this.newTrackId = '';
    this.newTrackTitle = '';
  }

  async syncPlaylist() {
    const testPlaylist = { tracks: [
      { id: '4JLcAU2xY90qTkTSNM1lUa', title: 'Nancy Sinatra - Bang Bang (My Baby Shot Me Down)' },
      { id: '3aw9iWUQ3VrPQltgwvN9Xu', title: 'Mary J. Blige - Family Affair' },
      { id: '1T8IRUJBga0JXioJZvxjBR', title: 'Rammstein - DEUTSCHLAND' },
      { id: '4pN7yDeBmmAud5lBtuCsVs', title: 'Darell, Farruko  - Caliente' },
      { id: '3spdoTYpuCpmq19tuD0bOe', title: 'Frank Sinatra - My Way (Expanded Version)' }
    ] };
    
    await this.performReq(() => this._accountService!.syncPlaylist(this.playlist));
    this.tracksToRemove = [];
    this.tracksToAdd = [];
  }
}
