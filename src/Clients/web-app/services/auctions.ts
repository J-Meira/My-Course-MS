"use server";

import { revalidatePath } from "next/cache";
import { FieldValues } from "react-hook-form";

import { fetchWrapper } from "@/libs";
import { Auction, Bid, PagedResult, ServiceError } from "@/types";

export const getAuctionsList = async (
  query: string
): Promise<PagedResult<Auction>> => {
  return await fetchWrapper.get(`search/${query}`);
};

export const updateAuctionTest = async (): Promise<string | ServiceError> => {
  const data = {
    mileage: Math.floor(Math.random() * 100000) + 1,
  };

  return await fetchWrapper.put(
    "auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c",
    data
  );
};

export const createAuction = async (
  data: FieldValues
): Promise<string | ServiceError> => {
  const res = await fetchWrapper.post("auctions", data);
  return res.error ? res : res.id;
};

export const getAuctionById = async (id: string): Promise<Auction> => {
  return await fetchWrapper.get(`auctions/${id}`);
};

export const updateAuction = async (
  data: FieldValues,
  id: string
): Promise<boolean | ServiceError> => {
  await fetchWrapper.put(`auctions/${id}`, data);
  revalidatePath(`/auctions/${id}`);
  return true;
};

export const deleteAuction = async (id: string) => {
  return await fetchWrapper.del(`auctions/${id}`);
};

export async function getBidsForAuction(
  id: string
): Promise<Bid[] | ServiceError> {
  return fetchWrapper.get(`bids/${id}`);
}

export async function placeBidForAuction(auctionId: string, amount: number) {
  return fetchWrapper.post(`bids?auctionId=${auctionId}&amount=${amount}`, {});
}
