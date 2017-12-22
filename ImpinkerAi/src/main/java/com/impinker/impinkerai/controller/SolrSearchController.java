package com.impinker.impinkerai.controller;


import com.impinker.impinkerai.service.SolrSearchService;
import com.impinker.impinkerai.util.ResultUtil;
import com.impinker.impinkerai.vo.ResultVo;
import lombok.extern.slf4j.Slf4j;
import lombok.val;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

@Slf4j
@RestController
public class SolrSearchController{

    //@Slf4j lombok 插件自动生成log 实例

    @Autowired
    SolrSearchService solrSearchService;

    @GetMapping("index")
    public String test(){

        return  ("search index");
    }

    @GetMapping("search/{keyword}")
    public ResultVo searchKey(@PathVariable("keyword")String keyword) throws Exception {


        val result=solrSearchService.search(keyword,1,10);
        //log.info(result);
        return ResultUtil.success(result);
        //return ResultUtil.error(ResultEnum.CART_EMPTY);
    }
}
