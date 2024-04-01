import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AddNewDeviceComponent } from './add-new-device/add-new-device.component';
import { DeviceService } from '../../core/services/http/device.service';
import { DeviceRequest } from '../../core/models/device-request';

@Component({
  selector: 'app-devices',
  standalone: true,
  imports: [],
  templateUrl: './devices.component.html',
  styleUrl: './devices.component.scss'
})
export class DevicesComponent {
  modalVisible: boolean = false;
  devices: any[] = [];
  deviceRequest: DeviceRequest = {
    adminId: 0
  };

  constructor(public dialog: MatDialog,
    private deviceService: DeviceService) { }

  ngOnInit(): void {
    this.getAll();
  }
  openDialog(): void {
    const dialogRef = this.dialog.open(AddNewDeviceComponent, {
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
  edit(device: any): void {
    const deviceId=device.id;
    this.deviceService.updateDevice(this.deviceRequest,deviceId).subscribe(() => {
      console.log('Device updated successfully');
      this.getAll();
    });
  }

  delete(device:any): void {
    const deviceId=device.id;
    this.deviceService.deleteDevice(this.deviceRequest.adminId,deviceId).subscribe(() => {
      console.log('Device deleted successfully');
      this.getAll();
    });

  }

  getAll(): void {
    this.deviceService.getCompanyDevices(this.deviceRequest.adminId).subscribe(devices => {
      this.devices = devices;
    });
  }
}