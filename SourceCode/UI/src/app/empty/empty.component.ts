import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableService } from '@core/services/observable.service';
import { BaseComponent } from '../base.component';

@Component({
  selector: 'app-empty',
  templateUrl: './empty.component.html',
  styleUrls: ['./empty.component.scss']
})
export class EmptyComponent extends BaseComponent implements OnInit {

  constructor(private observableService: ObservableService,
    private router: Router) {
    super();
  }

  ngOnInit(): void {
    this.rootFolderSubscription();
  }

  rootFolderSubscription() {
    this.sub$.sink = this.observableService.rootFolder$.subscribe(folder => {
      this.router.navigate(['/', folder.id]);
    });
  }
}
