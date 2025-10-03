import { Component, OnInit } from '@angular/core';
import { OnlineUser } from '@core/domain-classes/online-user';
import { UserAuth } from '@core/domain-classes/user-auth';
import { SecurityService } from '@core/security/security.service';
import { SignalrService } from '@core/services/signalr.service';
import { BaseComponent } from './base.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent extends BaseComponent implements OnInit {
    constructor(private signalrService: SignalrService, private securityService: SecurityService) {
        super();
    }
    ngOnInit() {
        this.signalrService.startConnection().then(resolve => {
            if (resolve) {
                this.signalrService.handleMessage();
                this.getAuthObj();
            }
        });
    }

    getAuthObj() {
        this.sub$.sink = this.securityService.securityObject$.subscribe((c: UserAuth) => {
            if (c) {
                const online: OnlineUser = {
                    email: c.email,
                    id: c.id,
                    connectionId: this.signalrService.connectionId
                };
                this.signalrService.addUser(online);
            }
        });
    }
}
