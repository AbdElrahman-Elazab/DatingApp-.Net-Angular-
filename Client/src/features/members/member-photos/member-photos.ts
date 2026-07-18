import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member, Photo } from '../../../types/members';
import { MemberService } from '../../../core/services/member-service';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { ImageUpload } from '../../../shared/image-upload/image-upload';
import { AccountService } from '../../../core/services/account-service';
import { User } from '../../../types/user';
import { StarButton } from "../../../shared/star-button/star-button";
import { DeleteButton } from "../../../shared/delete-button/delete-button";

@Component({
  selector: 'app-member-photos',
  imports: [AsyncPipe, ImageUpload, StarButton, DeleteButton],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css',
})
export class MemberPhotos implements OnInit {
  protected memberService = inject(MemberService);
  protected accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  protected photos = signal<Photo[]>([]);
  loading=signal<boolean>(false)

  ngOnInit(): void {
    const memberId = this.route.parent?.snapshot.paramMap.get('id');
    if (memberId) {
      this.memberService.getMemberPhotos(memberId).subscribe({
        next:(photos)=>{
          this.photos.set(photos);
        }
      });
    }
  }

onUploadPhoto(file:File){
this.loading.set(true);
this.memberService.uploadPhoto(file).subscribe({
  next:(photo)=>{
    this.memberService.editMode.set(false)
    this.loading.set(false);
    this.photos.update(photos=>[...photos,photo])
    
    if(!this.memberService.member()?.imageUrl)
      this.setMainLocalPhoto(photo)
  },
  error:(error)=>{
    console.log(`Error uploading photo ${error}`)
    this.loading.set(false)

  }
})
}

setMainPhoto(photo:Photo){
this.memberService.setMainPhoto(photo).subscribe({
  next:()=>{
this.setMainLocalPhoto(photo);
  }
})
}

deletePhoto(photo:Photo){
this.memberService.deletePhoto(photo).subscribe({
  next:()=>{
    this.photos.update(photos => photos.filter(p=>p.id!=photo.id))
  }
})
}

private setMainLocalPhoto(photo:Photo){
const currentUser=this.accountService.currentUser();
   if (currentUser) {
        this.accountService.setCurrentUser({
          ...currentUser,
          imageUrl: photo.url
        });
      }

    this.memberService.member.update(member=>({...member,imageUrl:photo.url})as Member)
}
}
