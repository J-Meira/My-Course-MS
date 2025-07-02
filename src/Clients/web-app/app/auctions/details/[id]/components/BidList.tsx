"use client";

import { useState, useEffect, useMemo } from "react";
import { User } from "next-auth";
import toast from "react-hot-toast";

import { DefaultsDisplay, Heading } from "@/components";

import { useBidStore } from "@/hooks";
import { numberWithCommas } from "@/libs";
import { getBidsForAuction } from "@/services";
import { Auction } from "@/types";

import { BidItem, BidForm } from ".";

type Props = {
  user: User | null;
  auction: Auction;
};

export const BidList = ({ user, auction }: Props) => {
  const [loading, setLoading] = useState(true);
  const { bids, open, setBids, setOpen } = useBidStore();
  const openForBids = new Date(auction.auctionEnd) > new Date();

  const highBid = useMemo(
    () =>
      bids.reduce(
        (prev, current) =>
          prev > current.amount
            ? prev
            : current.bidStatus.includes("Accepted")
            ? current.amount
            : prev,
        0
      ),
    [bids]
  );

  useEffect(() => {
    setLoading(true);
    getBidsForAuction(auction.id)
      .then((res) => {
        if (!Array.isArray(res)) {
          throw res.error;
        }
        setBids(res);
      })
      .catch((error) => {
        toast.error(error.message);
      })
      .finally(() => setLoading(false));
  }, [auction.id, setBids]);

  useEffect(() => {
    setOpen(openForBids);
  }, [openForBids, setOpen]);

  if (loading) return <span>Loading bids...</span>;

  return (
    <div className="rounded-lg shadow-lg">
      <div className="py-2 px-4 bg-white">
        <div className="sticky top-0 bg-white p-2">
          <Heading
            title={`Current high bid is $${numberWithCommas(highBid)}`}
          />
        </div>
      </div>

      <div className="overflow-auto h-[350px] flex flex-col-reverse px-2">
        {bids.length === 0 ? (
          <DefaultsDisplay
            title="No bids for this item"
            subtitle={open ? "Please feel free to make a bid" : ""}
          />
        ) : (
          bids.map((bid) => <BidItem key={bid.id} bid={bid} />)
        )}
      </div>

      <div className="px-2 pb-2 text-gray-500">
        {!open ? (
          <div className="flex items-center justify-center p-2 text-lg font-semibold">
            This auction has finished
          </div>
        ) : !user ? (
          <div className="flex items-center justify-center p-2 text-lg font-semibold">
            Please login to place a bid
          </div>
        ) : user && user.username === auction.seller ? (
          <div className="flex items-center justify-center p-2 text-lg font-semibold">
            You cannot bid on your own auction
          </div>
        ) : (
          <BidForm auctionId={auction.id} highBid={highBid} />
        )}
      </div>
    </div>
  );
};
