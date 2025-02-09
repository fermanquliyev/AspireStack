import { Component, EventEmitter, inject, OnInit, output, Output, ViewChild } from '@angular/core';
import { FormsModule, NgModel, NgModelGroup } from '@angular/forms';
import * as coreui from '@coreui/angular';
import {
  ApiService,
  CreateEditUserDto,
  RoleDto,
} from 'src/app/services/api-services/api-service-proxies';
import { DatePipe, NgIf } from '@angular/common';
import { UserDisplayCardComponent } from '../../shared/user-display-card/user-display-card.component';

@Component({
  selector: 'app-create-edit-user-modal',
  templateUrl: './create-edit-user-modal.component.html',
  styleUrls: ['./create-edit-user-modal.component.css'],
  imports: [
    FormsModule,
    coreui.RowComponent,
    coreui.ColComponent,
    coreui.TextColorDirective,
    coreui.CardComponent,
    coreui.CardHeaderComponent,
    coreui.CardBodyComponent,
    coreui.TableDirective,
    coreui.ModalComponent,
    coreui.ModalBodyComponent,
    coreui.ModalHeaderComponent,
    coreui.ModalFooterComponent,
    coreui.ModalToggleDirective,
    coreui.ButtonCloseDirective,
    coreui.ButtonDirective,
    coreui.PaginationComponent,
    coreui.FormDirective,
    coreui.FormControlDirective,
    coreui.FormLabelDirective,
    coreui.TabsComponent,
    coreui.TabsListComponent,
    coreui.TabContentComponent,
    coreui.TabsContentComponent,
    coreui.TabPanelComponent,
    coreui.TabDirective,
    coreui.ListGroupDirective,
    coreui.ListGroupItemDirective,
    coreui.FormCheckComponent,
    coreui.FormCheckLabelDirective,
    coreui.FormCheckInputDirective,
    coreui.BadgeComponent,
    coreui.AlertComponent,
    NgIf,
    DatePipe,
    UserDisplayCardComponent
  ],
})
export class CreateEditUserModalComponent implements OnInit {
  @ViewChild('createEditUserModal')
  public createEditUserModal!: coreui.ModalComponent;
  modalService = inject(coreui.ModalService);
  userDto: CreateEditUserDto = new CreateEditUserDto();
  allRoles: { role: RoleDto; selected: boolean }[] = [];
  @Output('onUserCreated') public onUserCreated: EventEmitter<CreateEditUserDto> = new EventEmitter();
  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.apiService.getAllRoles().subscribe((roles) => {
      this.allRoles = roles.map((role) => {
        return {
          role: role,
          selected: false,
        };
      });
    });
  }

  public open(id?: string) {
    if (id) {
      this.apiService.getUserById(id).subscribe((user) => {
        this.userDto = user;
        this.allRoles = this.allRoles.map((role) => {
          return {
            role: role.role,
            selected: this.userDto.roleIds?.find((r) => r === role.role.id)
              ? true
              : false,
          };
        });
        this.toggle(true);
      });
    } else {
      this.userDto = new CreateEditUserDto();
      this.allRoles.forEach((r) => (r.selected = false));
      this.toggle(true);
    }
  }

  public close() {
    this.modalService.toggle({
      modal: this.createEditUserModal,
      id: this.createEditUserModal.id,
      show: false,
    });
  }

  save() {
    this.userDto.roleIds = this.allRoles
      .filter((r) => r.selected)
      .map((r) => r.role.id!);
    if (this.userDto.id) {
      this.apiService.updateUser(this.userDto).subscribe(() => {
        this.allRoles.forEach((r) => (r.selected = false));
        this.onUserCreated.emit(this.userDto);
        this.close();
      });
    } else {
      this.apiService.createUser(this.userDto).subscribe(() => {
        this.allRoles.forEach((r) => (r.selected = false));
        this.onUserCreated.emit(this.userDto);
        this.close();
      });
    }
  }

  toggle(show: boolean) {
    this.modalService.toggle({
      modal: this.createEditUserModal,
      id: this.createEditUserModal.id,
      show: show,
    });
  }
}
