"use client";

import { useEffect, useState } from "react";

import qs from "query-string";

import { getAuctionsList } from "@/services";
import { DefaultsDisplay, AppPagination } from "@/components";
import { useAuctionStore, useParamsStore } from "@/hooks";

import { AuctionCard, Filters } from ".";

export const AuctionsList = () => {
  const [loading, setLoading] = useState(false);
  const { auctions, totalCount, pageCount, setData } = useAuctionStore();
  const {
    pageNumber,
    pageSize,
    searchTerm,
    orderBy,
    filterBy,
    seller,
    winner,
    setParams,
  } = useParamsStore();

  const url = qs.stringifyUrl({
    url: "",
    query: {
      pageNumber,
      pageSize,
      searchTerm,
      orderBy,
      filterBy,
      seller,
      winner,
    },
  });

  function setPageNumber(pageNumber: number) {
    setParams({ pageNumber });
  }

  useEffect(() => {
    setLoading(true);
    getAuctionsList(url).then((data) => {
      setLoading(false);
      setData(data);
    });
  }, [url]);

  if (loading) return <h3>Loading...</h3>;

  return (
    <>
      <Filters />
      {totalCount === 0 ? (
        <DefaultsDisplay showReset />
      ) : (
        <>
          <div className="grid grid-cols-4 gap-6">
            {auctions.map((auction) => (
              <AuctionCard auction={auction} key={auction.id} />
            ))}
          </div>
          <div className="flex justify-center mt-4">
            <AppPagination
              pageChanged={setPageNumber}
              currentPage={pageNumber}
              pageCount={pageCount}
            />
          </div>
        </>
      )}
    </>
  );
};
