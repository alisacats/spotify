import { delay } from "bluebird";
import { autoinject } from "aurelia-framework";
import { AccountsService } from "../api/accounts-service";
import { AccountBriefInfo } from "api/models";
import { Router } from 'aurelia-router';
import { Routes } from '../routing/routes';

@autoinject
export class AccountList {
  accounts: AccountBriefInfo[] = [];

  constructor(
    private readonly apiClient: AccountsService,
    private readonly router: Router
  ) {
    this.runFetchLoop();
  }

  async fetchData() {
    // const briefInfo = { accounts: [ { accountId: 1, email: '1', state: { isPlaying: true } }, { accountId: 2, email: '2', state: { isPlaying: false } } ] };
    const briefInfo = await this.apiClient.getBriefInfo();
    this.accounts = briefInfo.accounts;
  }

  async runFetchLoop() {
    while (true) {
      try {
        await this.fetchData();
      } catch { }
      await delay(1000);
    }
  }

  openAccount(account: AccountBriefInfo) {
    this.router.navigateToRoute(Routes.account.name, { accountId: account.accountId });
  }
}
