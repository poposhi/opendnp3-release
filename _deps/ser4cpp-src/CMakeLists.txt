cmake_minimum_required (VERSION 3.8)

project(ser4cpp VERSION 1.0.0)

set(is_root OFF)
if(${CMAKE_SOURCE_DIR} STREQUAL ${CMAKE_CURRENT_SOURCE_DIR})
	set(is_root ON)
endif()
set(SER4CPP_BUILD_TESTS ${is_root} CACHE BOOL "Build unit tests")

add_subdirectory(./src)

if(${SER4CPP_BUILD_TESTS})
    enable_testing()
    add_subdirectory(./test)
endif()
