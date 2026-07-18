export interface Member {
  id: string
  email: string
  gender: string
  dateOfBirth: string
  displayName: string
  created: string
  lastActive: string
  description?: string
  city: string
  country: string
  imageUrl?: string
}

export interface Photo {
  id: number
  url: string
  publicId?: any
  memberId: string
}
export type EditableMember={
    displayName: string
  description?: string
  city: string
  country: string
}
