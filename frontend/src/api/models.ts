export type AccountId = number

export interface AccountState {
  isPlaying: boolean
}

export interface AccountBriefInfo {
  accountId: AccountId
  email: string
  state: AccountState
}

export type TrackId = string

export interface Track {
  id: TrackId
  title: string
}

export interface AccountPlaylist {
  tracks: Track[]
}

export interface TrackStatistics {
  track: Track
  playCount: number
}



export interface GetBriefInfoResponse {
  accounts: AccountBriefInfo[]
}


export interface GetAccountPlaylistResponse {
  playlist: AccountPlaylist
}


export interface GetAccountStateResponse {
  state: AccountState
}


export interface StartAccountPlayingRequest { }

export interface StartAccountPlayingResponse {
  state: AccountState
}


export interface StopAccountPlayingRequest { }

export interface StopAccountPlayingResponse {
  state: AccountState
}


export interface GetAccountStatisticsResponse {
  statistics: TrackStatistics[]
}


export interface SyncAccountPlaylistRequest {
  playlist: AccountPlaylist
}

export interface SyncAccountPlaylistResponse { }
