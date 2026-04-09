import type { Dispatch, SetStateAction } from "react";
import type { CreateJobRequestDto } from "@/api/jobs/jobs.types";

export type JobFormSetter = Dispatch<SetStateAction<CreateJobRequestDto>>;

export function setStringValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: JobFormSetter,
  key: TKey,
  value: CreateJobRequestDto[TKey]
) {
  setFormValues((current) => ({
    ...current,
    [key]: value
  }));
}

export function setNullableStringValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: JobFormSetter,
  key: TKey,
  value: string
) {
  setFormValues((current) => ({
    ...current,
    [key]: value === "" ? null : value
  }));
}

export function setNullableNumberValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: JobFormSetter,
  key: TKey,
  value: string
) {
  setFormValues((current) => ({
    ...current,
    [key]: value === "" ? null : Number(value)
  }));
}

export function setBooleanValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: JobFormSetter,
  key: TKey,
  value: boolean
) {
  setFormValues((current) => ({
    ...current,
    [key]: value
  }));
}

export function setNullableBooleanValue<TKey extends keyof CreateJobRequestDto>(
  setFormValues: JobFormSetter,
  key: TKey,
  value: boolean
) {
  setFormValues((current) => ({
    ...current,
    [key]: value
  }));
}
