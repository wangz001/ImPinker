package com.lang.fblife;

import us.codecraft.webmagic.Spider;

public class FblifeMain {

	public static void main(String[] args) {

		TourPageStart();

		CulturePageStart();
	}

	public static void CulturePageStart() {
		Spider.create(new FblifeCulturePageProcessor())
				.addUrl("http://www.fblife.com/")
				.addPipeline(new FblifePipeline()).thread(5).run();
	}

	public static void TourPageStart() {
		Spider.create(new FbLifeTourPageProcessor())
				.addUrl("http://tour.fblife.com/")
				.addPipeline(new FblifePipeline()).thread(5).run();
	}

}
