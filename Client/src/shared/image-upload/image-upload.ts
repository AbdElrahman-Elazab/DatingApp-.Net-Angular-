import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css',
})
export class ImageUpload {
protected imageUrl=signal<string |ArrayBuffer | null | undefined>(null);
protected isDragable=false;
private filetoUpload:File|null=null;
upload=output<File>();
loading=input<boolean>(false);

onDragOver(event:DragEvent){
event.preventDefault();
event.stopPropagation();
this.isDragable=true;
}
onDragLeave(event:DragEvent){
event.preventDefault();
event.stopPropagation();
this.isDragable=false;
}
onDrop(event:DragEvent){
  event.preventDefault();
event.stopPropagation();
this.isDragable=false;

if(event.dataTransfer?.files.length){
const file=event.dataTransfer.files[0];
this.previewImage(file);
this.filetoUpload=file;

}
}
onCancle(){
  this.filetoUpload=null;
  this.imageUrl.set(null);

}
onUpdateFile(){
  if(this.filetoUpload)
  this.upload.emit(this.filetoUpload)
}

private previewImage(file:File){
const reader=new FileReader();
reader.onload=(e)=>this.imageUrl.set(e.target?.result);
reader.readAsDataURL(file)
}
}


