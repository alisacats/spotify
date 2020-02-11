import { GetBriefInfoResponse } from './models';
import { autoinject } from "aurelia-framework";
import { RestClient } from "./rest-client";
import { AccountService } from './account-service';

@autoinject
export class AccountsService {
  constructor(private readonly restClient: RestClient) { }

  async getBriefInfo() {
    return await this.restClient.get<GetBriefInfoResponse>('/accounts/get-brief-info');
  }
  
  getAccountService(accountId: number) {
    return new AccountService(this.restClient, accountId);
  }
}
