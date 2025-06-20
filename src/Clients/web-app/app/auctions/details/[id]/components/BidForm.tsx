"use client";

import { FieldValues, useForm } from "react-hook-form";
import toast from "react-hot-toast";

import { useBidStore } from "@/hooks";
import { numberWithCommas } from "@/libs";
import { placeBidForAuction } from "@/services";

type Props = {
  auctionId: string;
  highBid: number;
};

export const BidForm = ({ auctionId, highBid }: Props) => {
  const { register, handleSubmit, reset } = useForm();
  const { addBid } = useBidStore();

  const onSubmit = (data: FieldValues) => {
    if (data.amount < highBid) {
      reset();
      return toast.error(
        `Bid must be at least $${numberWithCommas(highBid + 1)}`
      );
    }

    placeBidForAuction(auctionId, +data.amount)
      .then((bid) => {
        reset();
        if (bid.error) throw bid.error;
        addBid(bid);
      })
      .catch((err) => toast.error(err.message));
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="flex items-center border-2 rounded-lg py-2"
    >
      <input
        type="number"
        {...register("amount")}
        className="input-custom text-sm text-gray-600"
        placeholder={`Enter your bid (minimum bid is $${numberWithCommas(
          highBid + 1
        )})`}
      />
    </form>
  );
};
