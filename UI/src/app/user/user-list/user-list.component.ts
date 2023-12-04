import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Router } from '@angular/router';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { OnlineUser } from '@core/domain-classes/online-user';
import { ResponseHeader } from '@core/domain-classes/response-header';
import { User } from '@core/domain-classes/user';
import { UserAuth } from '@core/domain-classes/user-auth';
import { UserResource } from '@core/domain-classes/user-resource';
import { SecurityService } from '@core/security/security.service';
import { SignalrService } from '@core/services/signalr.service';
import { ToastrService } from 'ngx-toastr';
import { merge } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { BaseComponent } from 'src/app/base.component';
import { ResetPasswordComponent } from '../reset-password/reset-password.component';
import { UserService } from '../user.service';
import { UserDataSource } from './user-datasource';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent extends BaseComponent implements OnInit, AfterViewInit {
  dataSource: UserDataSource;
  displayedColumns: string[] = ['action', 'email', 'firstName', 'phoneNumber', 'isAdmin', 'isActive', 'loginInto', 'size'];
  footerToDisplayed: string[] = ["footer"];
  userResource: UserResource;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  emilFilterCtl: FormControl = new FormControl('');
  fullNameFilterCtl: FormControl = new FormControl('');
  phoneNumberSearchFilterCtl: FormControl = new FormControl('');
  isActiveSearchFilterCtl: FormControl = new FormControl(true);
  isLoading: boolean = false;
  userAuth: UserAuth = null;
  constructor(
    private userService: UserService,
    private toastrService: ToastrService,
    private commonDialogService: CommonDialogService,
    private dialog: MatDialog,
    private router: Router,
    private securityService: SecurityService,
    private signalrService: SignalrService,
    private toastr: ToastrService) {
    super();
    this.userResource = new UserResource();
    this.userResource.pageSize = 10;
    this.userResource.orderBy = 'email desc'
  }

  ngOnInit(): void {
    this.getUserAuth();
    this.dataSource = new UserDataSource(this.userService);
    this.dataSource.loadUsers(this.userResource);
    this.getResourceParameter();
    this.filterLogic();
  }

  getUserAuth() {
    this.sub$.sink = this.securityService.securityObject$.subscribe(c => {
      this.userAuth = c;
    });
  }

  filterLogic() {
    this.sub$.sink = this.emilFilterCtl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(c => {
      this.userResource.email = c;
      this.userResource.skip = 0;
      this.dataSource.loadUsers(this.userResource);
    });

    this.sub$.sink = this.fullNameFilterCtl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(c => {
      if (c) {
        const name = c.trim().split(' ');
        this.userResource.first_name = name[0];
        if (name.length > 1)
          this.userResource.last_name = name[1];
        this.userResource.skip = 0;
      } else {
        this.userResource.first_name = '';
        this.userResource.last_name = '';
        this.userResource.skip = 0;
      }
      this.dataSource.loadUsers(this.userResource);
    });

    this.sub$.sink = this.phoneNumberSearchFilterCtl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(c => {
      this.userResource.phone_number = c;
      this.userResource.skip = 0;
      this.dataSource.loadUsers(this.userResource);
    });

    this.sub$.sink = this.isActiveSearchFilterCtl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(c => {
      this.userResource.is_active = c;
      this.userResource.skip = 0;
      this.dataSource.loadUsers(this.userResource);
    })

  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    this.sub$.sink = merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap((c: any) => {
          this.userResource.skip = this.paginator.pageIndex * this.paginator.pageSize;
          this.userResource.pageSize = this.paginator.pageSize;
          this.userResource.orderBy = this.sort.active + ' ' + this.sort.direction;
          this.dataSource.loadUsers(this.userResource);
        })
      )
      .subscribe();

  }

  deleteUser(user: User) {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(`'你确定要删除' ${user.email}`)
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.sub$.sink = this.userService.deleteUser(user.id)
            .subscribe(() => {
              this.toastrService.success('用户删除成功');
              this.paginator.pageIndex = 0;
              this.dataSource.loadUsers(this.userResource);
            });
        }
      });
  }

  getResourceParameter() {
    this.sub$.sink = this.dataSource.responseHeaderSubject$
      .subscribe((c: ResponseHeader) => {
        if (c) {
          this.userResource.pageSize = c.pageSize;
          this.userResource.skip = c.skip;
          this.userResource.totalCount = c.totalCount;
        }
      });
  }

  resetPassword(user: User): void {
    this.dialog.open(ResetPasswordComponent, {
      width: '350px',
      data: Object.assign({}, user)
    });
  }

  editUser(userId: string) {
    this.router.navigate(['/admin/manage', userId])
  }

  loginInto(user: User): void {
    this.isLoading = true;
    this.sub$.sink = this.securityService.userIntoLogin(user)
      .subscribe((c: UserAuth) => {
        const userInfo: OnlineUser = {
          email: c.email,
          id: c.id,
          connectionId: null
        }
        this.signalrService.addUser(userInfo);
        this.toastr.success('用户登录成功');
        this.isLoading = false;
        this.router.navigate(['/']);
      }, (err) => {
        this.isLoading = false;
      });
  }
}
