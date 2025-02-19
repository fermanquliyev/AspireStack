import { Component, EventEmitter, inject, OnInit, output, Output, ViewChild } from '@angular/core';
import { FormsModule, NgModel, NgModelGroup } from '@angular/forms';
import * as coreui from '@coreui/angular';
import {
  ApiService,
  CreateEditUserDto,
  RoleDto,
} from 'src/app/services/api-services/api-service-proxies';
import { DatePipe, NgIf } from '@angular/common';
// import { UserDisplayCardComponent } from '../../shared/user-display-card/user-display-card.component';


@Component({
  selector: 'app-create-edit-role-modal',
  templateUrl: './create-edit-role-modal.component.html',
  styleUrls: ['./create-edit-role-modal.component.css'],
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
    DatePipe
  ],
})
export class CreateEditRoleModalComponent implements OnInit {
  @ViewChild('createEditRoleModal')
  public createEditRoleModal!: coreui.ModalComponent;
  modalService = inject(coreui.ModalService);
  roleDto: RoleDto = new RoleDto();
  allPermissions: { [key: string]: boolean } = {};
  permissions: string[] = [];
  @Output('onRoleCreated') public onRoleCreated: EventEmitter<RoleDto> = new EventEmitter();
  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.apiService.getAllPermissions().subscribe((perms) => {
      this.permissions = Object.values(perms);
      this.permissions.forEach((p) => (this.allPermissions[p] = false));
    });
  }

  public open(id?: string) {
    if (id) {
      this.apiService.getRole(id).subscribe((role) => {
        this.roleDto = role;
        Object.keys(this.allPermissions).forEach((key) => {
          this.allPermissions[key] = role.permissions!.includes(key);
        });
        this.toggle(true);
      });
    } else {
      this.roleDto = new RoleDto();
      this.roleDto.permissions = [];
      Object.keys(this.allPermissions).forEach((key) => {
        this.allPermissions[key] = false;
      });
      this.toggle(true);
    }
  }

  public close() {
    this.modalService.toggle({
      modal: this.createEditRoleModal,
      id: this.createEditRoleModal.id,
      show: false,
    });
  }

  save() {
    this.roleDto.permissions = Object.keys(this.allPermissions).filter((key) => this.allPermissions[key]);
    if (this.roleDto.id) {
      this.apiService.updateRole(this.roleDto).subscribe(() => {
        Object.keys(this.allPermissions).forEach((key) => {
          this.allPermissions[key] = false;
        });
        this.onRoleCreated.emit(this.roleDto);
        this.close();
      });
    } else {
      this.apiService.createRole(this.roleDto).subscribe(() => {
        Object.keys(this.allPermissions).forEach((key) => {
          this.allPermissions[key] = false;
        });
        this.onRoleCreated.emit(this.roleDto);
        this.close();
      });
    }
  }

  toggle(show: boolean) {
    this.modalService.toggle({
      modal: this.createEditRoleModal,
      id: this.createEditRoleModal.id,
      show: show,
    });
  }
}
